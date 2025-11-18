using AMIProjectAPI.Helpers;
using AMIProjectAPI.Models;
using AMIProjectAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MetersController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        private readonly ILogger<MetersController> _logger;

        public MetersController(AmiprojectContext ctx, ILogger<MetersController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        // GET api/meters
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var user = HttpContext.User;

                if (user.IsUser())
                {
                    var all = await _ctx.Meters
                        .AsNoTracking()
                        .OrderBy(m => m.MeterSerialNo)
                        .ToListAsync();
                    return Ok(all);
                }

                var consumerId = user.GetConsumerId();
                if (consumerId.HasValue)
                {
                    var result = await _ctx.Meters
                        .AsNoTracking()
                        .Where(m => m.ConsumerId == consumerId.Value)
                        .OrderBy(m => m.MeterSerialNo)
                        .ToListAsync();
                    return Ok(result);
                }

                _logger.LogWarning("GetAll meters: caller not a User and ConsumerId claim missing/invalid.");
                return Forbid("ConsumerId claim missing or invalid.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load meters");
                return StatusCode(500, new { error = "Server error loading meters." });
            }
        }

        // GET api/meters/{serial}
        [HttpGet("{serial}")]
        public async Task<IActionResult> Get(string serial)
        {
            try
            {
                var meter = await _ctx.Meters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.MeterSerialNo == serial);

                if (meter == null) return NotFound(new { error = "Meter not found." });

                var user = HttpContext.User;
                if (user.IsUser()) return Ok(meter);

                var consumerId = user.GetConsumerId();
                if (consumerId.HasValue && consumerId.Value == meter.ConsumerId)
                    return Ok(meter);

                _logger.LogWarning("Get meter {Serial} forbidden for caller", serial);
                return Forbid("Not allowed to access this meter.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading meter {Serial}", serial);
                return StatusCode(500, new { error = "Server error reading meter." });
            }
        }

        // CREATE (admin only)
        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Create([FromBody] MeterCreateDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Meter payload is empty." });

            if (string.IsNullOrWhiteSpace(dto.MeterSerialNo))
                return BadRequest(new { field = "MeterSerialNo", message = "Serial is required." });

            var ipNorm = (dto.Ipaddress ?? "").Trim();
            if (!string.IsNullOrEmpty(ipNorm))
            {
                var existsIp = await _ctx.Meters.AnyAsync(m => m.Ipaddress == ipNorm);
                if (existsIp)
                    return Conflict(new { field = "Ipaddress", message = "IP address already exists." });
            }

            // uniqueness
            if (await _ctx.Meters.AnyAsync(m => m.MeterSerialNo == dto.MeterSerialNo))
                return Conflict(new { error = $"Meter with serial '{dto.MeterSerialNo}' already exists." });

            // validate consumer
            if (!await _ctx.Consumers.AnyAsync(c => c.ConsumerId == dto.ConsumerId))
                return BadRequest(new { error = $"ConsumerId {dto.ConsumerId} is invalid / not found." });

            // validate orgunit
            if (!await _ctx.OrgUnits.AnyAsync(o => o.OrgUnitId == dto.OrgUnitId))
                return BadRequest(new { error = $"OrgUnitId {dto.OrgUnitId} is invalid / not found." });

            var allowed = new[] { "Residential Tariff", "Commercial Tariff", "Factory Tariff" };
            if (!allowed.Contains(dto.Category))
                return BadRequest(new { field = "Category", message = "Invalid Category." });

            var m = new Meter
            {
                MeterSerialNo = dto.MeterSerialNo,
                ConsumerId = dto.ConsumerId,
                OrgUnitId = dto.OrgUnitId,
                Ipaddress = ipNorm,
                Iccid = dto.Iccid,
                Imsi = dto.Imsi,
                Manufacturer = dto.Manufacturer,
                Firmware = dto.Firmware,
                Category = dto.Category,
                InstallDate = dto.InstallDate,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status
            };

            _ctx.Meters.Add(m);
            try
            {
                await _ctx.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { serial = m.MeterSerialNo }, m);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error creating meter {Serial}", dto.MeterSerialNo);
                if (dbEx.InnerException != null)
                {
                    var inner = dbEx.InnerException.Message ?? "";
                    if (inner.IndexOf("Ipaddress", StringComparison.OrdinalIgnoreCase) >= 0
                        || inner.IndexOf("IX_Meters_Ipaddress", StringComparison.OrdinalIgnoreCase) >= 0
                        || inner.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return Conflict(new { field = "Ipaddress", message = "IP address already exists." });
                    }
                }
                return Conflict(new { error = "Database constraint prevented creating meter." });
            }
        }

        // UPDATE (admin only)
        [HttpPut("{serial}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Update(string serial, [FromBody] MeterUpdateDto dto)
        {
            var m = await _ctx.Meters.FirstOrDefaultAsync(x => x.MeterSerialNo == serial);
            if (m == null) return NotFound(new { error = "Meter not found." });

            if (!await _ctx.Consumers.AnyAsync(c => c.ConsumerId == dto.ConsumerId))
                return BadRequest(new { error = $"ConsumerId {dto.ConsumerId} is invalid / not found." });

            if (!await _ctx.OrgUnits.AnyAsync(o => o.OrgUnitId == dto.OrgUnitId))
                return BadRequest(new { error = $"OrgUnitId {dto.OrgUnitId} is invalid / not found." });

            var ipNorm = (dto.Ipaddress ?? "").Trim();
            if (!string.IsNullOrEmpty(ipNorm))
            {
                var exists = await _ctx.Meters.AnyAsync(x => x.Ipaddress == ipNorm && x.MeterSerialNo != serial);
                if (exists)
                    return Conflict(new { field = "Ipaddress", message = "IP address already exists." });
            }


            m.ConsumerId = dto.ConsumerId;
            m.OrgUnitId = dto.OrgUnitId;

            if (!string.IsNullOrWhiteSpace(dto.Ipaddress)) m.Ipaddress = ipNorm;
            if (!string.IsNullOrWhiteSpace(dto.Iccid)) m.Iccid = dto.Iccid;
            if (!string.IsNullOrWhiteSpace(dto.Imsi)) m.Imsi = dto.Imsi;
            if (!string.IsNullOrWhiteSpace(dto.Manufacturer)) m.Manufacturer = dto.Manufacturer;
            if (!string.IsNullOrWhiteSpace(dto.Firmware)) m.Firmware = dto.Firmware;

            if (!string.IsNullOrWhiteSpace(dto.Category))
            {
                var allowed = new[] { "Residential Tariff", "Commercial Tariff", "Factory Tariff" };
                if (!allowed.Contains(dto.Category)) return BadRequest(new { field = "Category", message = "Invalid Category." });
                m.Category = dto.Category;
            }

            if (dto.InstallDate.HasValue) m.InstallDate = dto.InstallDate.Value;
            if (!string.IsNullOrWhiteSpace(dto.Status)) m.Status = dto.Status;

            try
            {
                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error updating meter {Serial}", serial);
                if (dbEx.InnerException != null)
                {
                    var inner = dbEx.InnerException.Message ?? "";
                    if (inner.IndexOf("Ipaddress", StringComparison.OrdinalIgnoreCase) >= 0
                        || inner.IndexOf("IX_Meters_Ipaddress", StringComparison.OrdinalIgnoreCase) >= 0
                        || inner.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return Conflict(new { field = "Ipaddress", message = "IP address already exists." });
                    }
                }
                return Conflict(new { error = "Database constraint prevented updating meter." });
            }
        }

        // DELETE -> soft decommission (admin only)
        [HttpDelete("{serial}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Decommission(string serial)
        {
            var m = await _ctx.Meters.FindAsync(serial);
            if (m == null) return NotFound(new { error = "Meter not found." });

            m.Status = "Decommissioned";
            try
            {
                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error decommissioning meter {Serial}", serial);
                return Conflict(new { error = "Unable to decommission meter due to DB constraint." });
            }
        }
    }
}

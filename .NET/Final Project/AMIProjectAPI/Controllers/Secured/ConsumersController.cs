// File: AMIProjectAPI/Controllers/Secured/ConsumersController.cs
using AMIProjectAPI.Models;
using AMIProjectAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // allow authenticated principals; specific admin actions have UserPolicy
    public class ConsumersController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        private readonly ILogger<ConsumersController> _logger;

        public ConsumersController(AmiprojectContext ctx, ILogger<ConsumersController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        private async Task<int> ResolveConsumerIdFromClaimsAsync()
        {
            // Try multiple claim names (ConsumerId, consumerid, or numeric NameIdentifier)
            var claim = User.Claims.FirstOrDefault(c =>
                string.Equals(c.Type, "ConsumerId", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Type, "consumerid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase));

            if (claim != null && int.TryParse(claim.Value, out var cid) && cid > 0)
                return cid;

            // map by username using ConsumerLogin table (if present)
            var username = User.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(username))
            {
                var login = await _ctx.ConsumerLogins
                    .AsNoTracking()
                    .Include(cl => cl.Consumer)
                    .FirstOrDefaultAsync(cl => cl.Username == username);

                if (login?.Consumer != null) return login.Consumer.ConsumerId;
            }

            return 0;
        }

        // ------------------------------
        // Consumer-scoped read endpoints (existing)
        // ------------------------------
        [HttpGet("me/meters")]
        public async Task<IActionResult> GetMyMeters()
        {
            var consumerId = await ResolveConsumerIdFromClaimsAsync();
            if (consumerId <= 0)
            {
                _logger.LogWarning("ConsumerId claim missing or invalid for user {User}", User.Identity?.Name);
                return Forbid();
            }

            var meters = await _ctx.Meters
                .AsNoTracking()
                .Where(m => m.ConsumerId == consumerId)
                .OrderBy(m => m.MeterSerialNo)
                .Select(m => new
                {
                    m.MeterSerialNo,
                    Ipaddress = m.Ipaddress,
                    Iccid = m.Iccid,
                    Imsi = m.Imsi,
                    Manufacturer = m.Manufacturer,
                    Firmware = m.Firmware,
                    Category = m.Category,
                    InstallDate = m.InstallDate,
                    Status = m.Status,
                    m.ConsumerId,
                    m.OrgUnitId
                })
                .ToListAsync();

            return Ok(meters);
        }

        [HttpGet("me/bills")]
        public async Task<IActionResult> GetMyBills([FromQuery] string? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? meterId)
        {
            var consumerId = await ResolveConsumerIdFromClaimsAsync();
            if (consumerId <= 0) return Forbid();

            var meterIds = await _ctx.Meters
                .AsNoTracking()
                .Where(m => m.ConsumerId == consumerId)
                .Select(m => m.MeterSerialNo)
                .ToListAsync();

            var q = _ctx.Bills.AsNoTracking().Where(b => meterIds.Contains(b.MeterId));

            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(b => b.Status == status);
            if (!string.IsNullOrWhiteSpace(meterId)) q = q.Where(b => b.MeterId == meterId);
            if (from.HasValue) q = q.Where(b => b.MonthStartDate >= DateOnly.FromDateTime(from.Value.Date));
            if (to.HasValue) q = q.Where(b => b.MonthStartDate <= DateOnly.FromDateTime(to.Value.Date));

            var bills = await q.OrderByDescending(b => b.GeneratedAt)
                .Select(b => new
                {
                    b.BillId,
                    b.MeterId,
                    b.MonthStartDate,
                    b.MonthlyConsumptionkWh,
                    b.Category,
                    b.BaseRate,
                    b.SlabRate,
                    b.TaxRate,
                    b.Amount,
                    b.Status,
                    b.GeneratedAt
                }).ToListAsync();

            return Ok(bills);
        }

        [HttpGet("me/monthly")]
        public async Task<IActionResult> GetMyMonthly([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? meterId)
        {
            var consumerId = await ResolveConsumerIdFromClaimsAsync();
            if (consumerId <= 0) return Forbid();

            var meterIds = await _ctx.Meters
                .AsNoTracking()
                .Where(m => m.ConsumerId == consumerId)
                .Select(m => m.MeterSerialNo)
                .ToListAsync();

            var q = _ctx.MonthlyConsumptions.AsNoTracking().Where(mc => meterIds.Contains(mc.MeterId));

            if (!string.IsNullOrWhiteSpace(meterId)) q = q.Where(mc => mc.MeterId == meterId);
            if (from.HasValue) q = q.Where(mc => mc.MonthStartDate >= DateOnly.FromDateTime(from.Value.Date));
            if (to.HasValue) q = q.Where(mc => mc.MonthStartDate <= DateOnly.FromDateTime(to.Value.Date));

            var items = await q.OrderByDescending(mc => mc.MonthStartDate)
                .Select(mc => new
                {
                    mc.MeterId,
                    mc.MonthStartDate,
                    mc.ConsumptionkWh
                }).ToListAsync();

            return Ok(items);
        }


        // ------------------------------
        // NEW: Consumer-scoped meter write endpoints
        // ------------------------------
        /// <summary>
        /// Consumer creates a meter for themselves (consumerId is resolved from claims; provided ConsumerId in payload is ignored).
        /// </summary>
        [HttpPost("me/meters")]
        [Authorize(Policy = "ConsumerPolicy")]
        public async Task<IActionResult> CreateMeterForMe([FromBody] MeterCreateDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Meter payload is empty." });

            var consumerId = await ResolveConsumerIdFromClaimsAsync();
            if (consumerId <= 0) return Forbid();

            if (string.IsNullOrWhiteSpace(dto.MeterSerialNo))
                return BadRequest(new { field = "MeterSerialNo", message = "Serial is required." });

            // uniqueness
            if (await _ctx.Meters.AnyAsync(m => m.MeterSerialNo == dto.MeterSerialNo))
                return Conflict(new { error = $"Meter with serial '{dto.MeterSerialNo}' already exists." });

            // validate orgunit
            if (!await _ctx.OrgUnits.AnyAsync(o => o.OrgUnitId == dto.OrgUnitId))
                return BadRequest(new { error = $"OrgUnitId {dto.OrgUnitId} is invalid / not found." });

            var allowed = new[] { "Residential Tariff", "Commercial Tariff", "Factory Tariff" };
            if (!allowed.Contains(dto.Category))
                return BadRequest(new { field = "Category", message = "Invalid Category." });

            var m = new Meter
            {
                MeterSerialNo = dto.MeterSerialNo,
                ConsumerId = consumerId, // force to current consumer
                OrgUnitId = dto.OrgUnitId,
                Ipaddress = dto.Ipaddress,
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
                // return the created meter data (consumer can't access api/meters/{serial} if not authorized; return simple payload)
                return Created("", new
                {
                    m.MeterSerialNo,
                    m.ConsumerId,
                    m.OrgUnitId,
                    m.Ipaddress,
                    m.Iccid,
                    m.Imsi,
                    m.Manufacturer,
                    m.Firmware,
                    m.Category,
                    m.InstallDate,
                    m.Status
                });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error creating meter {Serial}", dto.MeterSerialNo);
                return Conflict(new { error = "Database constraint prevented creating meter." });
            }
        }

        /// <summary>
        /// Consumer updates a meter that belongs to them.
        /// </summary>
        [HttpPut("me/meters/{serial}")]
        [Authorize(Policy = "ConsumerPolicy")]
        public async Task<IActionResult> UpdateMeterForMe(string serial, [FromBody] MeterUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(serial)) return BadRequest(new { error = "Serial is required." });

            var consumerId = await ResolveConsumerIdFromClaimsAsync();
            if (consumerId <= 0) return Forbid();

            var m = await _ctx.Meters.FirstOrDefaultAsync(x => x.MeterSerialNo == serial);
            if (m == null) return NotFound(new { error = "Meter not found." });

            if (m.ConsumerId != consumerId) return Forbid("Not allowed to modify this meter.");

            // allow changing orgunit, ip, iccid, imsi, manuf, firmware, category, install date, status
            if (!await _ctx.OrgUnits.AnyAsync(o => o.OrgUnitId == dto.OrgUnitId))
                return BadRequest(new { error = $"OrgUnitId {dto.OrgUnitId} is invalid / not found." });

            m.OrgUnitId = dto.OrgUnitId;

            if (!string.IsNullOrWhiteSpace(dto.Ipaddress)) m.Ipaddress = dto.Ipaddress;
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
                return Conflict(new { error = "Database constraint prevented updating meter." });
            }
        }

        /// <summary>
        /// Consumer soft-decommissions (deletes) a meter that belongs to them.
        /// </summary>
        [HttpDelete("me/meters/{serial}")]
        [Authorize(Policy = "ConsumerPolicy")]
        public async Task<IActionResult> DecommissionMeterForMe(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial)) return BadRequest(new { error = "Serial is required." });

            var consumerId = await ResolveConsumerIdFromClaimsAsync();
            if (consumerId <= 0) return Forbid();

            var m = await _ctx.Meters.FindAsync(serial);
            if (m == null) return NotFound(new { error = "Meter not found." });

            if (m.ConsumerId != consumerId) return Forbid("Not allowed to decommission this meter.");

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



        // ------------------------------
        // Admin / UserPolicy endpoints (unchanged behaviour, explicitly require UserPolicy)
        // ------------------------------
        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetAll([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var query = _ctx.Consumers
                            .AsNoTracking()
                            .OrderBy(c => c.ConsumerId)
                            .AsQueryable();

            if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
            {
                var total = await query.CountAsync();
                var items = await query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToListAsync();

                return Ok(new
                {
                    total,
                    page,
                    pageSize,
                    items = items.Select(c => new
                    {
                        c.ConsumerId,
                        c.Name,
                        c.Address,
                        c.Phone,
                        c.Email,
                        c.Status,
                        c.CreatedAt,
                        c.CreatedBy,
                        c.UpdatedAt,
                        c.UpdatedBy
                    })
                });
            }

            var list = await query.ToListAsync();
            return Ok(list.Select(c => new
            {
                c.ConsumerId,
                c.Name,
                c.Address,
                c.Phone,
                c.Email,
                c.Status,
                c.CreatedAt,
                c.CreatedBy,
                c.UpdatedAt,
                c.UpdatedBy
            }));
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Create([FromBody] ConsumerCreateDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Request body required." });
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { field = "Name", message = "Name is required." });

            // If the client supplied username/password, validate them
            var wantsLogin = !string.IsNullOrWhiteSpace(dto.Username) || !string.IsNullOrWhiteSpace(dto.Password);

            if (wantsLogin)
            {
                if (string.IsNullOrWhiteSpace(dto.Username))
                    return BadRequest(new { field = "Username", message = "Username is required when creating a login." });
                if (string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest(new { field = "Password", message = "Password is required when creating a login." });

                // check username uniqueness in ConsumerLogins
                var usernameNormalized = dto.Username!.Trim();
                var usernameLower = usernameNormalized.ToLower();
                var exists = await _ctx.ConsumerLogins
                    .AsNoTracking()
                    .AnyAsync(cl => cl.Username.ToLower() == usernameLower);
                if (exists)
                    return BadRequest(new { field = "Username", message = "Username already exists." });
            }


            var entity = new Consumer
            {
                Name = dto.Name.Trim(),
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User?.Identity?.Name ?? "system"
            };

            _ctx.Consumers.Add(entity);
            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating consumer");
                return Conflict(new { error = "Database constraint violation while creating consumer." });
            }

            // If credentials requested, create ConsumerLogin
            if (wantsLogin)
            {
                try
                {
                    var usernameNormalized = dto.Username!.Trim();

                    // ***** SECURITY CHANGE: store password AS-IS (plaintext) instead of hashing *****
                    // This is intentionally insecure. Proceed only if you deliberately want plaintext storage.
                    var plainPassword = dto.Password;

                    var cl = new ConsumerLogin
                    {
                        ConsumerId = entity.ConsumerId,
                        Username = usernameNormalized,
                        Password = plainPassword, // <--- previously: BCrypt.Net.BCrypt.HashPassword(dto.Password);
                        LastLogin = null,
                        IsVerified = true,
                        Status = entity.Status ?? "Active"
                    };

                    _ctx.ConsumerLogins.Add(cl);
                    await _ctx.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                    _logger.LogError(dbEx, "DB error creating consumer login: {Msg}", msg);
                    return Conflict(new { field = "Username", message = "Unable to create login. Username may already exist." });
                }
            }

            return CreatedAtAction(nameof(Get), new { id = entity.ConsumerId }, new
            {
                entity.ConsumerId,
                entity.Name,
                entity.Address,
                entity.Phone,
                entity.Email,
                entity.Status,
                entity.CreatedAt,
                entity.CreatedBy
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Update(int id, [FromBody] ConsumerUpdateDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Request body required." });

            var entity = await _ctx.Consumers.FirstOrDefaultAsync(c => c.ConsumerId == id);
            if (entity == null) return NotFound(new { error = "Consumer not found." });

            entity.Name = dto.Name ?? entity.Name;
            entity.Address = dto.Address ?? entity.Address;
            entity.Phone = dto.Phone ?? entity.Phone;
            entity.Email = dto.Email ?? entity.Email;
            entity.Status = dto.Status ?? entity.Status;

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = User?.Identity?.Name ?? entity.UpdatedBy;

            try
            {
                // Persist changes to consumer table
                await _ctx.SaveChangesAsync();

                // --- also sync the status into ConsumerLogins (if a login exists) ---
                var login = await _ctx.ConsumerLogins.FirstOrDefaultAsync(cl => cl.ConsumerId == id);
                if (login != null)
                {
                    login.Status = entity.Status;
                    await _ctx.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating consumer {Id}", id);
                return Conflict(new { error = "Database constraint violation while updating consumer." });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _ctx.Consumers.FindAsync(id);
            if (entity == null)
                return NotFound(new { error = "Consumer not found." });
            _ctx.Consumers.Remove(entity);
            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _ctx.Consumers
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.ConsumerId == id);
            if (c == null)
                return NotFound(new { error = "Consumer not found." });

            return Ok(new
            {
                c.ConsumerId,
                c.Name,
                c.Address,
                c.Phone,
                c.Email,
                c.Status,
                c.CreatedAt,
                c.CreatedBy,
                c.UpdatedAt,
                c.UpdatedBy
            });
        }
    }
}

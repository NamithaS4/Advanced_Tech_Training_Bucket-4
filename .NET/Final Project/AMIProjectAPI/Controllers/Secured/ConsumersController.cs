using AMIProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
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

            // get meters for consumer
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
        [HttpGet]
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
                    var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                    var cl = new ConsumerLogin
                    {
                        ConsumerId = entity.ConsumerId,
                        Username = usernameNormalized,
                        Password = hashed,
                        LastLogin = null,
                        IsVerified = true,                    // requested: always true at creation
                        Status = entity.Status ?? "Active"
                    };

                    _ctx.ConsumerLogins.Add(cl);
                    await _ctx.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    // Possible unique constraint violation on username — return friendly error
                    var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                    _logger.LogError(dbEx, "DB error creating consumer login: {Msg}", msg);
                    // best to surface username conflict as field error
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
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating consumer {Id}", id);
                return Conflict(new { error = "Database constraint violation while updating consumer." });
            }

            return NoContent();
        }


        [HttpDelete("{id:int}")]
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

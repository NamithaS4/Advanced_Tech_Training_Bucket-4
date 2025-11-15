using AMIProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BillsController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        private readonly ILogger<BillsController> _logger;
        public BillsController(AmiprojectContext ctx, ILogger<BillsController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        private bool TryGetConsumerId(out int consumerId)
        {
            consumerId = 0;
            var c = User.Claims.FirstOrDefault(x => string.Equals(x.Type, "ConsumerId", StringComparison.OrdinalIgnoreCase));
            if (c == null) return false;
            return int.TryParse(c.Value, out consumerId);
        }

        // GET: api/bills
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? meterId, [FromQuery] string? status,
                                               [FromQuery] DateTime? from, [FromQuery] DateTime? to,
                                               [FromQuery] int? page, [FromQuery] int? pageSize,
                                               [FromQuery] int? consumerId) // optional consumerId to assist client
        {
            try
            {
                var q = _ctx.Bills
                    .AsNoTracking()
                    .Include(b => b.Meter)
                    .AsQueryable();

                // If caller supplied consumerId explicitly use it; otherwise check claims
                if (consumerId.HasValue)
                {
                    q = q.Where(b => b.Meter != null && b.Meter.ConsumerId == consumerId.Value);
                }
                else
                {
                    var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase)
                                                        && c.Value == "Consumer");
                    if (isConsumer)
                    {
                        if (!TryGetConsumerId(out var cid))
                        {
                            _logger.LogWarning("ConsumerId claim missing or invalid.");
                            return Forbid("ConsumerId claim missing or invalid.");
                        }
                        q = q.Where(b => b.Meter != null && b.Meter.ConsumerId == cid);
                    }
                }

                if (!string.IsNullOrWhiteSpace(meterId))
                    q = q.Where(b => b.MeterId == meterId);

                if (!string.IsNullOrWhiteSpace(status))
                    q = q.Where(b => b.Status == status);

                // The MVC Index uses MonthStartDate for month filtering in some places.
                // Keep supporting GeneratedAt filters (from/to) for backwards compatibility.
                if (from.HasValue)
                    q = q.Where(b => b.GeneratedAt >= from.Value);

                if (to.HasValue)
                    q = q.Where(b => b.GeneratedAt <= to.Value);

                q = q.OrderByDescending(b => b.GeneratedAt);

                // paging
                if (page.HasValue && pageSize.HasValue && page > 0 && pageSize > 0)
                {
                    var p = Math.Max(1, page.Value);
                    var ps = Math.Max(1, pageSize.Value);
                    var total = await q.CountAsync();
                    var items = await q.Skip((p - 1) * ps).Take(ps).ToListAsync();

                    var projected = items.Select(b => new
                    {
                        b.BillId,
                        MeterID = b.MeterId,
                        MonthStartDate = b.MonthStartDate,
                        MonthlyConsumptionkWh = b.MonthlyConsumptionkWh,
                        Category = b.Category ?? "",
                        BaseRate = b.BaseRate,
                        SlabRate = b.SlabRate,
                        TaxRate = b.TaxRate,
                        b.Amount,
                        b.Status,
                        GeneratedAt = b.GeneratedAt
                    });

                    return Ok(new
                    {
                        total,
                        page = p,
                        pageSize = ps,
                        items = projected
                    });
                }
                else
                {
                    var list = await q.ToListAsync();
                    var projected = list.Select(b => new
                    {
                        b.BillId,
                        MeterID = b.MeterId,
                        MonthStartDate = b.MonthStartDate,
                        MonthlyConsumptionkWh = b.MonthlyConsumptionkWh,
                        Category = b.Category ?? "",
                        BaseRate = b.BaseRate,
                        SlabRate = b.SlabRate,
                        TaxRate = b.TaxRate,
                        b.Amount,
                        b.Status,
                        GeneratedAt = b.GeneratedAt
                    });
                    return Ok(projected);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed loading bills");
                return StatusCode(500, new { error = "Server error loading bills." });
            }
        }

        // GET: api/bills/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var b = await _ctx.Bills
                .AsNoTracking()
                .Include(x => x.Meter)
                .FirstOrDefaultAsync(x => x.BillId == id);
            if (b == null) return NotFound(new { error = "Bill not found." });

            // consumer check
            var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
            if (isConsumer)
            {
                if (!TryGetConsumerId(out var cid)) return Forbid("ConsumerId claim missing or invalid.");
                if (b.Meter?.ConsumerId != cid) return Forbid("Not allowed to access this bill.");
            }

            return Ok(new
            {
                b.BillId,
                MeterID = b.MeterId,
                MonthStartDate = b.MonthStartDate,
                b.MonthlyConsumptionkWh,
                Category = b.Category ?? "",
                BaseRate = b.BaseRate,
                SlabRate = b.SlabRate,
                TaxRate = b.TaxRate,
                b.Amount,
                b.Status,
                b.GeneratedAt
            });
        }

        // POST: api/bills/pay/{id} -> mark bill as paid
        [HttpPost("pay/{id:int}")]
        public async Task<IActionResult> Pay(int id)
        {
            try
            {
                var bill = await _ctx.Bills
                    .Include(b => b.Meter)
                    .FirstOrDefaultAsync(b => b.BillId == id);

                if (bill == null) return NotFound(new { error = "Bill not found." });

                var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
                if (isConsumer)
                {
                    if (!TryGetConsumerId(out var cid)) return Forbid("ConsumerId claim missing or invalid.");
                    if (bill.Meter?.ConsumerId != cid) return Forbid("Not allowed to pay this bill.");
                }

                // If already paid or not pending, reject
                if (!string.Equals(bill.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { error = $"Bill status is '{bill.Status}' and cannot be paid." });

                bill.Status = "Paid";
                // Optionally record payment time — preserve GeneratedAt as bill generation time; you may add PaidAt field if needed

                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error paying bill {Id}", id);
                return StatusCode(500, new { error = "Database error while updating bill status." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error paying bill {Id}", id);
                return StatusCode(500, new { error = "Server error while processing payment." });
            }
        }
    }
}

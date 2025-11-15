using AMIProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // allow both admin/user and consumer authenticated principals
    public class ConsumptionController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        private readonly ILogger<ConsumptionController> _logger;

        public ConsumptionController(AmiprojectContext ctx, ILogger<ConsumptionController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        /// <summary>
        /// Attempt to resolve consumer id from incoming principal's claims.
        /// Returns null if not resolvable.
        /// </summary>
        private int? ResolveConsumerIdFromClaims()
        {
            // try common claim names
            var claim = User.Claims.FirstOrDefault(c =>
                string.Equals(c.Type, "ConsumerId", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Type, "consumerid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase));

            if (claim != null && int.TryParse(claim.Value, out var cid) && cid > 0)
                return cid;

            return null;
        }

        // GET api/consumption/daily?consumerId=&meterId=&from=yyyy-MM-dd&to=yyyy-MM-dd&page=&pageSize=
        [HttpGet("daily")]
        public async Task<IActionResult> GetDaily([FromQuery] int? consumerId, [FromQuery] string? meterId,
            [FromQuery] DateTime? from, [FromQuery] DateTime? to,
            [FromQuery] int? page, [FromQuery] int? pageSize)
        {
            try
            {
                // If caller is a consumer (UserType == Consumer) and no explicit consumerId was provided,
                // restrict results to the consumer resolved from claims.
                var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
                if (isConsumer && !consumerId.HasValue)
                {
                    var cid = ResolveConsumerIdFromClaims();
                    if (cid.HasValue)
                        consumerId = cid.Value;
                    else
                    {
                        // If consumer claim missing/invalid, forbid
                        _logger.LogWarning("Consumer identity missing or invalid for caller");
                        return Forbid("Consumer identity missing or invalid.");
                    }
                }

                // base query
                var q = _ctx.DailyConsumptions
                           .AsNoTracking()
                           .AsQueryable();

                // If consumerId provided (either explicitly or resolved from claims), restrict to that consumer's meters
                if (consumerId.HasValue)
                {
                    var meterIds = await _ctx.Meters
                        .Where(m => m.ConsumerId == consumerId.Value)
                        .Select(m => m.MeterSerialNo)
                        .ToListAsync();

                    q = q.Where(d => meterIds.Contains(d.MeterId));
                }

                if (!string.IsNullOrWhiteSpace(meterId))
                    q = q.Where(d => d.MeterId == meterId);

                // fetch to memory then apply date filters safely (handles DATE/DateOnly/DateTime DB types)
                var list = await q.OrderByDescending(d => d.ConsumptionDate).ToListAsync();

                IEnumerable<DailyRow> projected = list.Select(d => new DailyRow
                {
                    MeterId = d.MeterId,
                    ConsumptionDate = new DateTime(d.ConsumptionDate.Year, d.ConsumptionDate.Month, d.ConsumptionDate.Day),
                    ConsumptionkWh = d.ConsumptionkWh
                });

                if (from.HasValue)
                    projected = projected.Where(x => x.ConsumptionDate.Date >= from.Value.Date);
                if (to.HasValue)
                    projected = projected.Where(x => x.ConsumptionDate.Date <= to.Value.Date);

                var total = projected.Count();

                // paging
                if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
                {
                    var p = Math.Max(1, page.Value);
                    var ps = Math.Max(1, pageSize.Value);
                    var items = projected.Skip((p - 1) * ps).Take(ps).ToList();
                    return Ok(new { total, page = p, pageSize = ps, items });
                }

                return Ok(projected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDaily failed");
                return StatusCode(500, new { error = "Server error" });
            }
        }

        // GET api/consumption/monthly?consumerId=&meterId=&from=yyyy-MM-dd&to=yyyy-MM-dd&page=&pageSize=
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthly([FromQuery] int? consumerId, [FromQuery] string? meterId,
            [FromQuery] DateTime? from, [FromQuery] DateTime? to,
            [FromQuery] int? page, [FromQuery] int? pageSize)
        {
            try
            {
                // If caller is a consumer (UserType == Consumer) and no explicit consumerId was provided,
                // restrict results to the consumer resolved from claims.
                var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
                if (isConsumer && !consumerId.HasValue)
                {
                    var cid = ResolveConsumerIdFromClaims();
                    if (cid.HasValue)
                        consumerId = cid.Value;
                    else
                    {
                        _logger.LogWarning("Consumer identity missing or invalid for caller");
                        return Forbid("Consumer identity missing or invalid.");
                    }
                }

                var q = _ctx.MonthlyConsumptions
                           .AsNoTracking()
                           .AsQueryable();

                if (consumerId.HasValue)
                {
                    var meterIds = await _ctx.Meters
                        .Where(m => m.ConsumerId == consumerId.Value)
                        .Select(m => m.MeterSerialNo)
                        .ToListAsync();
                    q = q.Where(m => meterIds.Contains(m.MeterId));
                }

                if (!string.IsNullOrWhiteSpace(meterId))
                    q = q.Where(m => m.MeterId == meterId);

                var list = await q.OrderByDescending(m => m.MonthStartDate).ToListAsync();

                // project and normalize MonthStartDate to DateTime (safe whether DB property is DateOnly or DateTime)
                var projected = list.Select(m => new MonthlyRow
                {
                    MeterId = m.MeterId,
                    MonthStartDate = new DateTime(m.MonthStartDate.Year, m.MonthStartDate.Month, m.MonthStartDate.Day),
                    ConsumptionkWh = m.ConsumptionkWh
                });

                if (from.HasValue)
                    projected = projected.Where(x => x.MonthStartDate.Date >= from.Value.Date);
                if (to.HasValue)
                    projected = projected.Where(x => x.MonthStartDate.Date <= to.Value.Date);

                var total = projected.Count();

                if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
                {
                    var p = Math.Max(1, page.Value);
                    var ps = Math.Max(1, pageSize.Value);
                    var items = projected.Skip((p - 1) * ps).Take(ps).ToList();
                    return Ok(new { total, page = p, pageSize = ps, items });
                }

                return Ok(projected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMonthly failed");
                return StatusCode(500, new { error = "Server error" });
            }
        }

        // small projection types returned by API
        private class DailyRow
        {
            public string MeterId { get; set; } = "";
            public DateTime ConsumptionDate { get; set; }
            public decimal ConsumptionkWh { get; set; }
        }

        private class MonthlyRow
        {
            public string MeterId { get; set; } = "";
            public DateTime MonthStartDate { get; set; }
            public decimal ConsumptionkWh { get; set; }
        }
    }
}

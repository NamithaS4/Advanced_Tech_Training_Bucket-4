using AMIProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
    public class TariffsController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        private readonly ILogger<TariffsController> _logger;

        public TariffsController(AmiprojectContext ctx, ILogger<TariffsController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        // GET: api/tariffs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _ctx.Tariffs.AsNoTracking()
                .OrderBy(t => t.TariffId)
                .ToListAsync();
            return Ok(list);
        }

        // GET: api/tariffs/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var tariff = await _ctx.Tariffs.AsNoTracking().FirstOrDefaultAsync(t => t.TariffId == id);
            if (tariff == null) return NotFound(new { error = "Tariff not found." });
            return Ok(tariff);
        }

        // POST: api/tariffs
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Tariff t)
        {
            if (t == null) return BadRequest(new { error = "Request body required." });

            var incomingName = (t.TariffName ?? "").Trim();
            if (string.IsNullOrEmpty(incomingName))
                return BadRequest(new { error = "TariffName is required." });

            // Check duplicate (case-insensitive)
            var exists = await _ctx.Tariffs
                .AsNoTracking()
                .AnyAsync(x => x.TariffName != null && x.TariffName.Trim().ToLower() == incomingName.ToLower());

            if (exists)
                return Conflict(new { error = $"A tariff with name '{incomingName}' already exists." });

            var entity = new Tariff
            {
                TariffName = incomingName,
                EffectiveFrom = t.EffectiveFrom,
                EffectiveTo = t.EffectiveTo,
                BaseRate = t.BaseRate,
                TaxRate = t.TaxRate
            };

            _ctx.Tariffs.Add(entity);
            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error creating tariff");
                return Conflict(new { error = "Unable to create tariff due to database constraint." });
            }

            return CreatedAtAction(nameof(Get), new { id = entity.TariffId }, entity);
        }

        // PUT: api/tariffs/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Tariff t)
        {
            if (t == null) return BadRequest(new { error = "Request body required." });
            if (id != t.TariffId) return BadRequest(new { error = "Id mismatch." });

            var entity = await _ctx.Tariffs.FindAsync(id);
            if (entity == null) return NotFound(new { error = "Tariff not found." });

            var incomingName = (t.TariffName ?? "").Trim();
            if (string.IsNullOrEmpty(incomingName))
                return BadRequest(new { error = "TariffName is required." });

            // Check duplicate excluding current entity
            var duplicate = await _ctx.Tariffs
                .AsNoTracking()
                .AnyAsync(x => x.TariffId != id
                               && x.TariffName != null
                               && x.TariffName.Trim().ToLower() == incomingName.ToLower());

            if (duplicate)
                return Conflict(new { error = $"A tariff with name '{incomingName}' already exists." });

            // update fields
            entity.TariffName = incomingName;
            entity.EffectiveFrom = t.EffectiveFrom;
            entity.EffectiveTo = t.EffectiveTo;
            entity.BaseRate = t.BaseRate;
            entity.TaxRate = t.TaxRate;

            try
            {
                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException dbConcEx)
            {
                _logger.LogError(dbConcEx, "Concurrency error updating tariff id {Id}", id);
                return Conflict(new { error = "Concurrency error updating record." });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error updating tariff id {Id}", id);
                return Conflict(new { error = "Database constraint violation while updating tariff." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating tariff id {Id}", id);
                return StatusCode(500, new { error = "Unexpected server error while updating tariff." });
            }
        }
    }
}

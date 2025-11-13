using AMIProjectAPI.Models;
using AMIProjectAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
    public class TariffSlabsController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        private readonly ILogger<TariffSlabsController> _logger;

        public TariffSlabsController(AmiprojectContext ctx, ILogger<TariffSlabsController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        // GET api/tariffslabs/{tariffId}
        [HttpGet("{tariffId:int}")]
        public async Task<IActionResult> GetByTariff(int tariffId)
        {
            var list = await _ctx.TariffSlabs
                .Where(s => s.TariffId == tariffId)
                .OrderBy(s => s.FromKwh)
                .AsNoTracking()
                .ToListAsync();

            var projected = list.Select(s => new
            {
                s.SlabId,
                s.TariffId,
                s.FromKwh,
                s.ToKwh,
                s.RatePerKwh
            });

            return Ok(projected);
        }

        // GET api/tariffslabs/slab/{slabId}
        [HttpGet("slab/{slabId:int}")]
        public async Task<IActionResult> GetSlab(int slabId)
        {
            var s = await _ctx.TariffSlabs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SlabId == slabId);

            if (s == null) return NotFound(new { error = "Slab not found." });

            return Ok(new
            {
                s.SlabId,
                s.TariffId,
                s.FromKwh,
                s.ToKwh,
                s.RatePerKwh
            });
        }

        // POST api/tariffslabs
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TariffSlabDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Request body required." });

            // Basic validation
            if (dto.TariffId <= 0) return BadRequest(new { field = "TariffId", message = "TariffId is required and must be > 0." });
            if (dto.FromKwh < 0 || dto.ToKwh <= dto.FromKwh) return BadRequest(new { field = "FromKwh", message = "Invalid slab range (To must be greater than From)." });

            // ensure tariff exists
            var tariffExists = await _ctx.Tariffs.AnyAsync(t => t.TariffId == dto.TariffId);
            if (!tariffExists) return BadRequest(new { field = "TariffId", message = $"Tariff with id {dto.TariffId} does not exist." });

            // overlapping check
            bool overlaps = await _ctx.TariffSlabs
                .AnyAsync(s => s.TariffId == dto.TariffId &&
                               !(dto.ToKwh <= s.FromKwh || dto.FromKwh >= s.ToKwh));
            if (overlaps) return Conflict(new { error = "Overlapping slab range for this tariff." });

            var entity = new TariffSlab
            {
                TariffId = dto.TariffId,
                FromKwh = dto.FromKwh,
                ToKwh = dto.ToKwh,
                RatePerKwh = dto.RatePerKwh
            };

            _ctx.TariffSlabs.Add(entity);
            try
            {
                await _ctx.SaveChangesAsync();
                return Ok(new { message = "Slab created.", slabId = entity.SlabId });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error creating slab");
                return Conflict(new { error = "Database constraint prevented creating slab." });
            }
        }

        // PUT api/tariffslabs/{slabId}
        [HttpPut("{slabId:int}")]
        public async Task<IActionResult> Update(int slabId, [FromBody] TariffSlabDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Request body required." });

            var slab = await _ctx.TariffSlabs.FirstOrDefaultAsync(s => s.SlabId == slabId);
            if (slab == null) return NotFound(new { error = "Slab not found." });

            if (dto.TariffId <= 0) return BadRequest(new { field = "TariffId", message = "TariffId is required and must be > 0." });
            if (dto.FromKwh < 0 || dto.ToKwh <= dto.FromKwh) return BadRequest(new { field = "FromKwh", message = "Invalid slab range (To must be greater than From)." });

            var tariffExists = await _ctx.Tariffs.AnyAsync(t => t.TariffId == dto.TariffId);
            if (!tariffExists) return BadRequest(new { field = "TariffId", message = $"Tariff with id {dto.TariffId} does not exist." });

            // overlapping check excluding current slab
            bool overlaps = await _ctx.TariffSlabs
                .AnyAsync(s => s.TariffId == dto.TariffId && s.SlabId != slabId &&
                               !(dto.ToKwh <= s.FromKwh || dto.FromKwh >= s.ToKwh));
            if (overlaps) return Conflict(new { error = "Overlapping slab range for this tariff." });

            slab.TariffId = dto.TariffId;
            slab.FromKwh = dto.FromKwh;
            slab.ToKwh = dto.ToKwh;
            slab.RatePerKwh = dto.RatePerKwh;

            try
            {
                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error updating slab id {Id}", slabId);
                return Conflict(new { error = "Database constraint prevented updating slab." });
            }
        }

        // DELETE api/tariffslabs/{slabId}
        [HttpDelete("{slabId:int}")]
        public async Task<IActionResult> Delete(int slabId)
        {
            var e = await _ctx.TariffSlabs.FindAsync(slabId);
            if (e == null) return NotFound(new { error = "Slab not found." });

            _ctx.TariffSlabs.Remove(e);
            try
            {
                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error deleting slab id {Id}", slabId);
                return Conflict(new { error = "Unable to delete slab due to database constraint." });
            }
        }
    }
}

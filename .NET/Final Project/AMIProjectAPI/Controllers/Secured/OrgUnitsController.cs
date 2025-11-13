using AMIProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // both User & Consumer can read; only User can add
    public class OrgUnitsController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        private readonly ILogger<OrgUnitsController> _logger;

        public OrgUnitsController(AmiprojectContext ctx, ILogger<OrgUnitsController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _ctx.OrgUnits
                .AsNoTracking()
                .OrderBy(u => u.Zone).ThenBy(u => u.Substation).ThenBy(u => u.Feeder).ThenBy(u => u.Dtr)
                .ToListAsync();

            var projected = list.Select(u => new
            {
                u.OrgUnitId,
                u.Zone,
                u.Substation,
                u.Feeder,
                u.Dtr
            });

            return Ok(projected);
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Create([FromBody] OrgUnit dto)
        {
            if (dto == null)
                return BadRequest(new { error = "Request body is required." });

            // normalize inputs for comparison
            string zone = (dto.Zone ?? "").Trim();
            string sub = (dto.Substation ?? "").Trim();
            string fed = (dto.Feeder ?? "").Trim();
            string dtr = (dto.Dtr ?? "").Trim();

            if (string.IsNullOrEmpty(zone) || string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(fed) || string.IsNullOrEmpty(dtr))
            {
                return BadRequest(new { error = "Zone, Substation, Feeder and DTR are required." });
            }

            // Check for exact duplicate (case-insensitive)
            bool exists = await _ctx.OrgUnits.AnyAsync(o =>
                (o.Zone ?? "").Trim().ToLower() == zone.ToLower() &&
                (o.Substation ?? "").Trim().ToLower() == sub.ToLower() &&
                (o.Feeder ?? "").Trim().ToLower() == fed.ToLower() &&
                (o.Dtr ?? "").Trim().ToLower() == dtr.ToLower()
            );

            if (exists)
            {
                return Conflict(new { error = $"Org unit already exists: {zone} › {sub} › {fed} › {dtr}" });
            }

            var entity = new OrgUnit
            {
                Zone = zone,
                Substation = sub,
                Feeder = fed,
                Dtr = dtr
            };

            _ctx.OrgUnits.Add(entity);

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error saving orgunit");
                return StatusCode(500, new { error = "Database error saving org unit." });
            }

            return CreatedAtAction(nameof(GetAll), new { id = entity.OrgUnitId }, new
            {
                entity.OrgUnitId,
                entity.Zone,
                entity.Substation,
                entity.Feeder,
                entity.Dtr
            });
        }
    }
}

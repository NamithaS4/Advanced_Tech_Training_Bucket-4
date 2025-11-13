using AMIProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
    public class TodRulesController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        public TodRulesController(AmiprojectContext ctx) => _ctx = ctx;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _ctx.TodRules.AsNoTracking().ToListAsync();
            return Ok(rules.Select(r => new {
                r.TodRuleId,
                r.Name,
                r.StartTime,
                r.EndTime,
                r.PeakType,
                r.Multiplier,
                r.Status,
                r.CreatedAt,
                r.CreatedBy,
                r.UpdatedAt,
                r.UpdatedBy
            }));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var r = await _ctx.TodRules.AsNoTracking().FirstOrDefaultAsync(x => x.TodRuleId == id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodRule dto)
        {
            if (dto == null) return BadRequest();
            var r = new TodRule
            {
                Name = dto.Name,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                PeakType = dto.PeakType,
                Multiplier = dto.Multiplier,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User?.Identity?.Name ?? "system"
            };
            _ctx.TodRules.Add(r);
            await _ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = r.TodRuleId }, r);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TodRule dto)
        {
            var r = await _ctx.TodRules.FirstOrDefaultAsync(x => x.TodRuleId == id);
            if (r == null) return NotFound();
            if (!string.IsNullOrWhiteSpace(dto.Name)) r.Name = dto.Name;
            r.StartTime = dto.StartTime;
            r.EndTime = dto.EndTime;
            if (!string.IsNullOrWhiteSpace(dto.PeakType)) r.PeakType = dto.PeakType;
            if (dto.Multiplier > 0) r.Multiplier = dto.Multiplier;
            if (!string.IsNullOrWhiteSpace(dto.Status)) r.Status = dto.Status;
            r.UpdatedAt = DateTime.UtcNow;
            r.UpdatedBy = User?.Identity?.Name ?? r.UpdatedBy;
            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _ctx.TodRules.FirstOrDefaultAsync(x => x.TodRuleId == id);
            if (r == null) return NotFound();
            r.Status = "Inactive";
            r.UpdatedAt = DateTime.UtcNow;
            r.UpdatedBy = User?.Identity?.Name ?? r.UpdatedBy;
            await _ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}

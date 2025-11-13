using AMIProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "UserPolicy")]
public class DashboardController : ControllerBase
{
    private readonly AmiprojectContext _ctx;
    public DashboardController(AmiprojectContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // load basic collections
        var meters = await _ctx.Meters.AsNoTracking().ToListAsync();
        var consumers = await _ctx.Consumers.AsNoTracking().ToListAsync();
        var users = await _ctx.Users.AsNoTracking().ToListAsync();
        var tariffs = await _ctx.Tariffs.AsNoTracking().ToListAsync();

        // Counts
        var totalMeters = meters.Count;
        var activeMeters = meters.Count(m => string.Equals(m.Status, "Active", StringComparison.OrdinalIgnoreCase));

        var totalConsumers = consumers.Count;
        var activeConsumers = consumers.Count(c => string.Equals(c.Status, "Active", StringComparison.OrdinalIgnoreCase));

        var totalUsers = users.Count;
        var activeUsers = users.Count(u => string.Equals(u.Status, "Active", StringComparison.OrdinalIgnoreCase));

        var tariffPlans = tariffs.Count;

        // Recent consumers: prefer UpdatedAt, else CreatedAt.
        // Ensure we cast CreatedAt to nullable if it is not nullable on the model.
        var recentConsumers = consumers
            .OrderByDescending(c => (DateTime?)(c.UpdatedAt ?? (c.CreatedAt as DateTime? ?? (DateTime?)c.CreatedAt)))
            .Take(5)
            .Select(c => new
            {
                Name = c.Name,
                Email = c.Email,
                // expose the best timestamp as "UpdatedAt" (may be CreatedAt if UpdatedAt null)
                CreatedAt = (DateTime?)(c.UpdatedAt ?? (c.CreatedAt as DateTime? ?? (DateTime?)c.CreatedAt))
            })
            .ToList();

        // Recent users: prefer LastLogin (your User.cs has LastLogin nullable), fallback to null
        var recentUsers = users
            .OrderByDescending(u => u.LastLogin ?? DateTime.MinValue)
            .Take(5)
            .Select(u => new
            {
                Username = u.Username,
                DisplayName = u.DisplayName,
                LastLogin = u.LastLogin
            })
            .ToList();

        // Return aggregated object the frontend can consume
        return Ok(new
        {
            TotalMeters = totalMeters,
            ActiveMeters = activeMeters,
            TotalConsumers = totalConsumers,
            ActiveConsumers = activeConsumers,
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TariffPlans = tariffPlans,
            // manufacturers / dtrs removed because your context doesn't contain those DbSets
            RecentConsumers = recentConsumers,
            RecentUsers = recentUsers
        });
    }


}

using AMIProjectAPI.Dtos;
using AMIProjectAPI.DTOs;
using AMIProjectAPI.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserPolicy")]
    public class UsersController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AmiprojectContext ctx, ILogger<UsersController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _ctx.Users
                .AsNoTracking()
                .OrderBy(u => u.UserId)
                .ToListAsync();

            return Ok(users.Select(u => new
            {
                u.UserId,
                u.Username,
                u.DisplayName,
                u.Email,
                u.Phone,
                u.LastLogin,
                u.Status
            }));
        }

        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var u = await _ctx.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id);
            if (u == null) return NotFound(new { error = "User not found." });

            return Ok(new
            {
                u.UserId,
                u.Username,
                u.DisplayName,
                u.Email,
                u.Phone,
                u.Status,
                u.LastLogin
            });
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Request body required." });
            if (string.IsNullOrWhiteSpace(dto.Username)) return BadRequest(new { error = "Username is required." });
            if (string.IsNullOrWhiteSpace(dto.Password)) return BadRequest(new { error = "Password is required." });

            var username = dto.Username.Trim();

            var exists = await _ctx.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username.ToLower() == username.ToLower());

            if (exists) return Conflict(new { error = "Username already exists." });

            // Hash password with bcrypt and store
            string hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = username,
                DisplayName = dto.DisplayName?.Trim(),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status.Trim(),
                LastLogin = null,
                Password = hashed
            };

            _ctx.Users.Add(user);
            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "DB error creating user: {Msg}", msg);
                return StatusCode(500, new { error = "Database error creating user." });
            }

            return CreatedAtAction(nameof(Get), new { id = user.UserId }, new
            {
                user.UserId,
                user.Username,
                user.DisplayName,
                user.Email,
                user.Phone,
                user.Status
            });
        }

        // PUT: api/users/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            if (dto == null) return BadRequest(new { error = "Request body required." });

            var u = await _ctx.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (u == null) return NotFound(new { error = "User not found." });

            if (!string.IsNullOrWhiteSpace(dto.DisplayName)) u.DisplayName = dto.DisplayName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Email)) u.Email = dto.Email.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Phone)) u.Phone = dto.Phone.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Status)) u.Status = dto.Status.Trim();

            try
            {
                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "DB error updating user {Id}: {Msg}", id, msg);
                return StatusCode(500, new { error = "Database error updating user." });
            }
        }

        // DELETE: api/users/{id}  (hard delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _ctx.Users.FindAsync(id);
            if (u == null) return NotFound(new { error = "User not found." });

            _ctx.Users.Remove(u);
            try
            {
                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                // probably FK constraint -> return friendly conflict
                var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "DB error deleting user {Id}: {Msg}", id, msg);
                return Conflict(new { error = "Unable to delete user. It may be referenced by other records." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting user {Id}", id);
                return StatusCode(500, new { error = "Unexpected server error while deleting user." });
            }
        }
    }
}

using CollegeAppAPI.Models;
using CollegeAppAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CollegeAppController : ControllerBase
    {
        private readonly IStudentRepository _repo;
        private readonly CollegeDbContext _context;

        public CollegeAppController(IStudentRepository repo, CollegeDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        // ✅ Get all students
        [HttpGet("All")]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

        // ✅ Add new student
        [HttpPost("Add")]
        public async Task<IActionResult> Add(Student s)
        {
            try
            {
                var existing = await _context.Students.FirstOrDefaultAsync(x => x.RollNumber == s.RollNumber);
                if (existing != null)
                    return BadRequest(new { message = "❌ Roll number already exists. Please use a unique roll number." });

                var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == s.CourseId);
                if (!courseExists)
                    return BadRequest(new { message = "⚠️ Invalid Course ID. Please select an existing course." });

                await _repo.AddAsync(s);
                return Ok(new { message = "✅ Student added successfully!" });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.Message.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase) == true)
                    return BadRequest(new { message = "⚠️ Duplicate primary key detected." });
                if (dbEx.InnerException?.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) == true)
                    return BadRequest(new { message = "⚠️ Invalid course reference. Please choose a valid course." });

                return BadRequest(new { message = "⚠️ Database constraint error: " + dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "⚠️ Unexpected error: " + ex.Message });
            }
        }

        // ✅ Update student
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Student s)
        {
            try
            {
                var existing = await _context.Students
                    .FirstOrDefaultAsync(x => x.RollNumber == s.RollNumber && x.StudentId != s.StudentId);

                if (existing != null)
                    return BadRequest(new { message = "⚠️ Roll number already exists. Please use a unique roll number." });

                var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == s.CourseId);
                if (!courseExists)
                    return BadRequest(new { message = "⚠️ Invalid Course ID. Please select an existing course." });

                await _repo.UpdateAsync(s);
                return Ok(new { message = "✅ Student updated successfully!" });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.Message.Contains("UNIQUE KEY", StringComparison.OrdinalIgnoreCase) == true)
                    return BadRequest(new { message = "⚠️ Roll number must be unique." });
                if (dbEx.InnerException?.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) == true)
                    return BadRequest(new { message = "⚠️ Invalid course reference. Please choose a valid course." });

                return BadRequest(new { message = "⚠️ Database constraint error: " + dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "⚠️ Unexpected error: " + ex.Message });
            }
        }

        // ✅ Delete student
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                return Ok(new { message = "✅ Student deleted successfully!" });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) == true)
                    return BadRequest(new { message = "⚠️ Cannot delete this student — it is linked with another record." });

                return BadRequest(new { message = "⚠️ Database constraint error: " + dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "⚠️ Unexpected error: " + ex.Message });
            }
        }

        // ✅ Get all courses
        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }

        // ✅ Get single course
        [HttpGet("GetCourse/{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "⚠️ Course not found." });
            return Ok(course);
        }

        // ✅ Add Course
        [HttpPost("AddCourse")]
        public async Task<IActionResult> AddCourse(Course course)
        {
            try
            {
                // 🆕 Validate semester
                if (course.Semester <= 0)
                    return BadRequest(new { message = "⚠️ Semester must be greater than 0." });

                // 🆕 Check duplicate course code
                var existing = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == course.CourseCode);
                if (existing != null)
                    return BadRequest(new { message = "⚠️ Course code already exists. Please use a unique code." });

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return Ok(new { message = "✅ Course added successfully!" });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.Message.Contains("UNIQUE KEY", StringComparison.OrdinalIgnoreCase) == true)
                    return BadRequest(new { message = "⚠️ Course code must be unique." });

                return BadRequest(new { message = "⚠️ Database constraint error: " + dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "⚠️ Unexpected error: " + ex.Message });
            }
        }

        // ✅ Update Course
        [HttpPut("UpdateCourse")]
        public async Task<IActionResult> UpdateCourse(Course course)
        {
            try
            {
                // 🆕 Validate semester
                if (course.Semester <= 0)
                    return BadRequest(new { message = "⚠️ Semester must be greater than 0." });

                // 🆕 Duplicate check
                var existing = await _context.Courses
                    .FirstOrDefaultAsync(c => c.CourseCode == course.CourseCode && c.CourseId != course.CourseId);

                if (existing != null)
                    return BadRequest(new { message = "⚠️ Course code already exists." });

                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
                return Ok(new { message = "✅ Course updated successfully!" });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.Message.Contains("UNIQUE KEY", StringComparison.OrdinalIgnoreCase) == true)
                    return BadRequest(new { message = "⚠️ Course code must be unique." });

                return BadRequest(new { message = "⚠️ Database constraint error: " + dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "⚠️ Unexpected error: " + ex.Message });
            }
        }

        // ✅ Delete Course
        [HttpDelete("DeleteCourse/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return NotFound(new { message = "⚠️ Course not found." });

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                return Ok(new { message = "✅ Course deleted successfully!" });
            }
            catch (DbUpdateException dbEx)
            {
                // ✅ Detect FK constraint violation safely (without relying on FK name)
                if (dbEx.InnerException?.Message.Contains("DELETE statement conflicted", StringComparison.OrdinalIgnoreCase) == true &&
                    dbEx.InnerException?.Message.Contains("dbo.Student", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return BadRequest(new { message = "⚠️ Cannot delete this course because it is assigned to one or more students." });
                }

                // ✅ Default error fallback
                return BadRequest(new { message = "⚠️ Database constraint error: " + dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "⚠️ Unexpected error: " + ex.Message });
            }
        }





        // (You can keep DeleteStudent below if used elsewhere)
        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                    return NotFound(new { message = "⚠️ Student not found." });

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return Ok(new { message = "✅ Student deleted successfully!" });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) == true)
                    return BadRequest(new { message = "⚠️ This student record is linked and cannot be deleted." });

                return BadRequest(new { message = "⚠️ Database constraint error: " + dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "⚠️ Unexpected error: " + ex.Message });
            }
        }
    }
}

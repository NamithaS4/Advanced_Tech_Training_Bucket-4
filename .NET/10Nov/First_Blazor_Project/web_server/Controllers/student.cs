using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using web_server.Models;

namespace web_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class student : ControllerBase
  
        {
            private readonly BlazorTutorialContext _context;

            public student(BlazorTutorialContext context)
            {
                _context = context;
            }

            // ✅ GET ALL STUDENTS
            [HttpGet("Get")]
            public async Task<ActionResult<IEnumerable<Student>>> Get()
            {
                return await _context.Students.ToListAsync();
            }

            // ✅ GET BY ID
            [HttpGet("{id}", Name = "GetStudentById")]
            public async Task<ActionResult<Student>> GetStudentById(int id)
            {
                if (id <= 0)
                    return BadRequest("Invalid ID");

                var student = await _context.Students
                                            .FirstOrDefaultAsync(s => s.Id == id);

                if (student == null)
                    return NotFound($"Student with ID {id} not found.");

                return Ok(student);
            }

            // ✅ GET BY NAME
            [HttpGet("GetByName/{name}")]
            public async Task<ActionResult<Student>> GetByName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest("Name cannot be empty");

                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());

                if (student == null)
                    return NotFound($"No student found with name {name}");

                return Ok(student);
            }

            // ✅ CREATE
            [HttpPost("Create")]
            public async Task<ActionResult<Student>> CreateStudent([FromBody] Student model)
            {
                if (model == null)
                    return BadRequest("Invalid data");

                var student = new Student
                {
                    Name = model.Name,
                    Age = model.Age,
                    Birthday = model.Birthday
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                return CreatedAtRoute("GetStudentById", new { id = student.Id }, student);
            }

            // ✅ UPDATE
            [HttpPut("Update/{id}")]
            public async Task<ActionResult<Student>> UpdateStudent(int id, [FromBody] Student model)
            {
                if (model == null || id <= 0)
                    return BadRequest();

                var existing = await _context.Students.FindAsync(id);

                if (existing == null)
                    return NotFound($"Student with ID {id} not found");

                existing.Name = model.Name;
                existing.Age = model.Age;
                existing.Birthday = model.Birthday;

                await _context.SaveChangesAsync();

                return Ok(existing);
            }

            // ✅ DELETE
            [HttpDelete("Delete/{id}")]
            public async Task<ActionResult> DeleteStudent(int id)
            {
                if (id <= 0)
                    return BadRequest("Invalid ID");

                var student = await _context.Students.FindAsync(id);

                if (student == null)
                    return NotFound($"Student with ID {id} not found");

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                return Ok(true);
            }
        }
    }


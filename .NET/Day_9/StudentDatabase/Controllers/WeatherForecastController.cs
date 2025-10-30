using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDatabase.Models;
using System.Runtime.CompilerServices;

namespace StudentDatabase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly CollegeContext _dbcontext;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, CollegeContext dbcontext)
        {
            _logger = logger;
            _dbcontext = dbcontext;
        }

        [HttpGet(Name = "GetStudent")]
        public async Task<ActionResult<IEnumerable<Student>>> getstudents()
        {
            var student = await _dbcontext.Students.ToListAsync();

            var students = _dbcontext.Students.Select(s => new Student()
            {
                StudentId = s.StudentId,
                Name = s.Name,
                Email = s.Email,
            }).ToListAsync();
            return Ok(student);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
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
        public IEnumerable<Student> Get()
        {
            return _dbcontext.Student;
        }
    }
}

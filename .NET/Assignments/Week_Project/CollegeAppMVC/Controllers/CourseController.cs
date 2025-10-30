using Microsoft.AspNetCore.Mvc;

namespace CollegeAppMVC.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

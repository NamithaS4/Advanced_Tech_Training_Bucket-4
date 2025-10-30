using Microsoft.AspNetCore.Mvc;

namespace CollegeAppMVC.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index() => View();
    }
}

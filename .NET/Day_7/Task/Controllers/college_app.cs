using Microsoft.AspNetCore.Mvc;

namespace Task.Controllers
{
    public class college_app : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

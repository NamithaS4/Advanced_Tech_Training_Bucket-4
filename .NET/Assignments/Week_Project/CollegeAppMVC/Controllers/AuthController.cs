using Microsoft.AspNetCore.Mvc;

namespace CollegeAppMVC.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login() => View();
    }
}

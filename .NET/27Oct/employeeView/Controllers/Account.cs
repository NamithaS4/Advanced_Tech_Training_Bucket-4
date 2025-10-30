using Microsoft.AspNetCore.Mvc;

namespace employeeView.Controllers
{
    public class Account : Controller
    {
        public IActionResult login()
        {
            return View();
        }
    }
}
using College_App.MyLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace College_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Logme : ControllerBase
    {
        private readonly IMyLogger _mylogger;


        public Logme(IMyLogger mylogger)
        {
            _mylogger = mylogger;
        }


        [HttpGet]
        public ActionResult index()
        {
            _mylogger.Log("index method started");


            return Ok();
        }
    }
}

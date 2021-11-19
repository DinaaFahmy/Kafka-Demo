using EmailSenderDemo.MQ;
using Microsoft.AspNetCore.Mvc;

namespace EmailSenderDemo.Controllers
{
    public class HomeController : Controller
    {
        public MQ_Service _mqService;
        public HomeController(MQ_Service mqService)
        {
            _mqService = mqService;
        }
        public IActionResult Index()
        {
            return Ok("Demo is running");
        }
    }
}

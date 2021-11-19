using Microsoft.AspNetCore.Mvc;
using MQ.Models;
using MQ.MQServices;
using System;
using System.Threading.Tasks;

namespace Producer.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {

        private readonly MQService _mqService;

        public HomeController(MQService mqService)
        {
            _mqService = mqService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Produce()
        {
            QueueEmailMessage message = new QueueEmailMessage
            {
                CreatedDate = DateTime.Now,
                Email = new Email()
                {
                    To = { "dina.fahmy@flairstech.com" },
                    Subject = "Test Kafka",
                    Body = "Test Kafka Works.",
                },
                SMTPConfiguration = new SMTPConfiguration
                {
                    Port = 587,
                    Host = "smtp.gmail.com",
                    Mail = "email@gmail.com",  //Add email here
                    Password = "password",  //Add password here
                    DisplayName = "Test Kafka",
                    QuotaType = QuotaType.Minute,
                    QuotaValue = 1
                },
            };
            await _mqService.ProduceEmailMessage(message);
            return RedirectToAction("Index");
        }
    }
}

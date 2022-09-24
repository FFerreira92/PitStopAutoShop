using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace PitStopAutoShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMailHelper _mailHelper;
       

        public HomeController(ILogger<HomeController> logger, IMailHelper mailHelper)
        {
            _logger = logger;
            _mailHelper = mailHelper;
          
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            var model = new ContactFormViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactFormViewModel model)
        {
            if (ModelState.IsValid)
            {

                var message = $"<p> New customer contact request <br/><br/><b>Name: <b/>{model.Name}<br/><b>Phone Number: </b>{model.PhoneNumber}<br/>" +
                    $"<b>Email: </b> {model.Email}<br/><b>Plate Number: </b>{model.PlateNumber}<br/><b>Message: </b>{model.Message}<br/><br/>Please refer to this customer as soon as possible. </br>" +
                    $"Your Pitstop management team.";

                var response = await _mailHelper.SendContactEmailAsync(model.Email,"Contact request",message,model.Name);

                if (response.IsSuccess)
                {
                    ViewBag.Message = "Your contact request was sent with success! We will get in touch with you as soon as possible.";                   
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ViewBag.Message = "There was a problem sending your contact request. Please try again.";
                    return View(model);
                }

            }

            return View(model);
        }


        public IActionResult CarCare()
        {
            return View();
        }

        public IActionResult Brakes()
        {
            return View();
        }

        public IActionResult Diagnostics()
        {
            return View();
        }

        public IActionResult Electrical()
        {
            return View();
        }

        public IActionResult ACService()
        {
            return View();
        }

        public IActionResult Engine()
        {
            return View();
        }

        public IActionResult Maintenance()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Electronic()
        {
            return View();
        }

        public IActionResult Undercar()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

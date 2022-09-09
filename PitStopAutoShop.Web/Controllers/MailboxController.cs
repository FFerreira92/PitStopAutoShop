using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Helpers;
using System.Collections.Generic;

namespace PitStopAutoShop.Web.Controllers
{
    public class MailboxController : Controller
    {
        private readonly IMailHelper _mailHelper;       
        

        public MailboxController(IMailHelper mailHelper)
        {
            _mailHelper = mailHelper;          
        }

        public IActionResult Index()
        {

         


            return View();
        }



    }
}

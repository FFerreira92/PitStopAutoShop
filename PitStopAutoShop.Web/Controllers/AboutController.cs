using Microsoft.AspNetCore.Mvc;

namespace PitStopAutoShop.Web.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

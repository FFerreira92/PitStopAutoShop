using Microsoft.AspNetCore.Mvc;

namespace PitStopAutoShop.Web.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

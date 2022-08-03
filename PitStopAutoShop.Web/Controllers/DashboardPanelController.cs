using Microsoft.AspNetCore.Mvc;

namespace PitStopAutoShop.Web.Controllers
{
    public class DashboardPanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

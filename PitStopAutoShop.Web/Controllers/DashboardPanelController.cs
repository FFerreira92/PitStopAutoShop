using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Repositories;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Controllers
{
    public class DashboardPanelController : Controller
    {
        private readonly IWorkOrderRepository _workOrderRepository;

        public DashboardPanelController(IWorkOrderRepository workOrderRepository)
        {
            _workOrderRepository = workOrderRepository;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [Route("DashboardPanel/GetOpenedWorkOrders")]
        public async Task<JsonResult> GetOpenedWorkOrders()
        {
            var OpenedWorkOrders = await _workOrderRepository.GetOpenedWorkOrdersAsync();
            var json = Json(OpenedWorkOrders);
            return json;
        }
    }
}

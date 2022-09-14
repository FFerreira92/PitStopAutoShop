using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Controllers
{
    public class DashboardPanelController : Controller
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserHelper _userHelper;

        public DashboardPanelController(IWorkOrderRepository workOrderRepository, IInvoiceRepository invoiceRepository,IUserHelper userHelper)
        {
            _workOrderRepository = workOrderRepository;
            _invoiceRepository = invoiceRepository;
            _userHelper = userHelper;
        }
        public async Task<IActionResult> Index()
        {
            //this month sales graphic data
            var thisMonthsales = await _invoiceRepository.GetMonthlySales(DateTime.UtcNow.Month);

            ViewBag.MonthlySales = thisMonthsales;
            ViewBag.MonthName = thisMonthsales.FirstOrDefault().Month;
            string[] color1 = { "#efcfe3" };
            string[] color2 = { "#ea9ab2" };
            string[] color3 = { "#e27396" };
            ViewBag.color1 = color1;
            ViewBag.color2 = color2;
            ViewBag.color3 = color3;

            //all months overal sales graphic data

            var overallMonthsSales = await _invoiceRepository.GetYearSalesByMonthAsync(DateTime.UtcNow.Year);

            ViewBag.OverallMonthsSales = overallMonthsSales;                     

            ViewBag.totalUsers = await _userHelper.GetTotalUsersAsync();

            ViewBag.activeWorkOrders = await _workOrderRepository.GetActiveWorkOrdersNumber();

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


        [HttpPost]
        [Route("DashboardPanel/GetChartUserData")]
        public async Task<JsonResult> GetChartUserData()
        {
            var registeredUserData = await _userHelper.GetUsersChartDataAsync();
            var json = Json(registeredUserData);
            return json;
        }


        [HttpPost]
        [Route("DashboardPanel/GetWorkOrdersChart")]
        public async Task<JsonResult> GetWorkOrdersChart()
        {
            var workOrdersData = await _workOrderRepository.GetWorkOrdersChartAsync(DateTime.UtcNow.Month);
            var json = Json(workOrdersData);
            return json;
        }


    }
}

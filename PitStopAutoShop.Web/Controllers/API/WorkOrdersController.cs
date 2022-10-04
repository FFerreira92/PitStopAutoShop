using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Repositories;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController : Controller
    {
        private readonly IWorkOrderRepository _workOrderRepository;

        public WorkOrdersController(IWorkOrderRepository workOrderRepository)
        {
            _workOrderRepository = workOrderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkOrders(string plateNumber)
        {
            var results = await _workOrderRepository.GetAllWorkOrdersByPlateNumberAsync(plateNumber);
            var result = Ok(results);            
            return result;
        }

    }
}

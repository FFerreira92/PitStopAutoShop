using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Controllers
{
    public class EstimatesController : Controller
    {
        private readonly IEstimateRepository _estimateRepository;
        private readonly IServiceRepository _serviceRepository;

        public EstimatesController(IEstimateRepository estimateRepository, IServiceRepository serviceRepository)
        {
            _estimateRepository = estimateRepository;
            _serviceRepository = serviceRepository;
        }

        public IActionResult Index()
        {
            var estimates =  _estimateRepository.GetAllEstimates();

            return View(estimates);
        }
    }
}

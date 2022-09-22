using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Repositories;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AboutController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Technicians()
        {
            var employees = await _employeeRepository.GetTechniciansEmployeesAsync();

            return View(employees);
        }

    }


   

}

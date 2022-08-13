using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace PitStopAutoShop.Web.Controllers
{
    public class EstimatesController : Controller
    {
        private readonly IEstimateRepository _estimateRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;

        public EstimatesController(IEstimateRepository estimateRepository, IServiceRepository serviceRepository,
                                   ICustomerRepository customerRepository, IVehicleRepository vehicleRepository,
                                   IUserHelper userHelper, IFlashMessage flashMessage)
        {
            _estimateRepository = estimateRepository;
            _serviceRepository = serviceRepository;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
        }

        public IActionResult Index()
        {
            var estimates =  _estimateRepository.GetAllEstimates();

            return View(estimates);
        }



        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(id.Value);

            if (vehicle == null)
            {
                return NotFound();
            }

            var userDetailTemps = await _estimateRepository.GetEstimateDetailTempWithVehicleIdAsync(this.User.Identity.Name, vehicle);

            var listmodel = await _estimateRepository.GetDetailTempsAsync(userDetailTemps.VehicleId, userDetailTemps.CustomerId);

            return View(listmodel);

            //var currentUserDetailTemps = await _estimateRepository.GetEstimateDetailTempAsync(this.User.Identity.Name);

            //var model = await _estimateRepository.GetDetailTempsAsync(currentUserDetailTemps.VehicleId, currentUserDetailTemps.CustomerId);
            //return View(model);
        }
      

        public async Task<IActionResult> AddService(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(id.Value);


            var estimateDetailTemp = await _estimateRepository.GetEstimateDetailTempWithVehicleIdAsync(this.User.Identity.Name, vehicle);            

            if(estimateDetailTemp == null)
            {
                return NotFound();
            }

            var model = new AddServiceToEstimateViewModel
            {
                Quantity = 1,
                Services = _serviceRepository.GetComboServices(),
                VehicleId = estimateDetailTemp.VehicleId,
                CustomerId = estimateDetailTemp.CustomerId,
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddService(AddServiceToEstimateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _serviceRepository.AddServiceToEstimateAsync(model, this.User.Identity.Name);
                return RedirectToAction("Create",new {id = model.VehicleId});
            }

            return View(model);
        }

        public async Task<IActionResult> SelectVehicle(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetCustomerWithUserByIdAsync(id.Value);


            if(customer == null)
            {
                return NotFound();
            }

            var customerVehicles = _vehicleRepository.GetCustomerVehiclesAsync(customer.Id);

            if(customerVehicles == null)
            {
                _flashMessage.Warning("The selected customer has no vehicles in his name. Try adding a vehicle to the customer first.");
                return RedirectToAction("Index", "Costumer");
            }

            var model = new AddCustomerAndVehicleToEstimateViewModel
            {
                CustomerId = customer.Id,
                Vehicles = _vehicleRepository.GetComboVehicles(customer.Id),
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> SelectVehicle(AddCustomerAndVehicleToEstimateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _customerRepository.GetCustomerWithUserByIdAsync(model.CustomerId);

                if(customer == null)
                {
                    return NotFound();
                }

                var vehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(model.VehicleId);

                if (vehicle == null)
                    return NotFound();

                var employeeUSer = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);


                var estimateDetailTemp = new EstimateDetailTemp
                {
                    CustomerId = model.CustomerId,
                    VehicleId = model.VehicleId,
                    User = employeeUSer                    
                };

                try
                {
                    await _estimateRepository.CreateEstimateDetailTemp(estimateDetailTemp);
                    return RedirectToAction(nameof(Create),new { id = estimateDetailTemp.VehicleId });                   
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error creating the estimate. " + ex.InnerException.Message);
                    return View(model);                    
                }              
                                             
            }
            return View(model);
        }

        public async Task<IActionResult> Increase(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var estimateDetailTemp =  await _estimateRepository.GetEstimateDetailTempByIdAsync(id.Value);
            
            if(estimateDetailTemp == null)
            {
                _flashMessage.Danger("there was an error increasing the quantity");
                return RedirectToAction("Create");
            }

            await _estimateRepository.ModifyEstimateDetailTempQuantityAsync(id.Value, 1);
            return RedirectToAction("Create", new {id = estimateDetailTemp.VehicleId});
        }

        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estimateDetailTemp = await _estimateRepository.GetEstimateDetailTempByIdAsync(id.Value);

            if (estimateDetailTemp == null)
            {
                _flashMessage.Danger("there was an error decreasing the quantity");
                return RedirectToAction("Create");
            }

            await _estimateRepository.ModifyEstimateDetailTempQuantityAsync(id.Value, -1);
            return RedirectToAction("Create", new { id = estimateDetailTemp.VehicleId });
        }

        public async Task<IActionResult> DeleteItem(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var estimateDetailTemp = await _estimateRepository.GetEstimateDetailTempByIdAsync(id.Value);
            var vehicleid = estimateDetailTemp.VehicleId;
            if (estimateDetailTemp == null)
            {
                _flashMessage.Danger("there was an error deleting the service");
                return RedirectToAction("Create");
            }

            await _estimateRepository.DeleteDetailTempAsync(id.Value);
            return RedirectToAction("Create", new
            {
                id = vehicleid
            });
        }


        public async Task<IActionResult> ConfirmEstimate()
        {

            var workingEstimateDetail = await _estimateRepository.GetEstimateDetailTempAsync(this.User.Identity.Name);

            var response = await _estimateRepository.ConfirmEstimateAsync(this.User.Identity.Name,workingEstimateDetail.CustomerId,workingEstimateDetail.VehicleId);

            if (response)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Create",new {id = workingEstimateDetail.VehicleId});
        }


        /* --------   Tentativa de adicionar cliente e viatura na mesma view (falhada conforme queria, não gostei do outcome...mas funciona se tiver de ser.)*/

        //public IActionResult AddCustomer()
        //{
        //    var model = new AddCustomerAndVehicleToEstimateViewModel
        //    {
        //         Customers = _customerRepository.GetComboCustomers(),
        //         Vehicles = _vehicleRepository.GetComboVehicles(0)
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddCustomer(AddCustomerAndVehicleToEstimateViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var customer = await _customerRepository.GetCustomerWithUserByIdAsync(model.CustomerId);

        //        if(customer == null)
        //        {
        //            return NotFound();
        //        }

        //        var vehicle = await _vehicleRepository.GetByIdAsync(model.VehicleId);

        //        if(vehicle == null)
        //        {
        //            return NotFound();
        //        }

        //        var employeeUser = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

        //        if (employeeUser == null)
        //        {
        //            return NotFound();
        //        }

        //        var estimate = new Estimate
        //        {
        //            Vehicle = vehicle,
        //            Customer = customer,
        //            CreatedBy = employeeUser,
        //            EstimateDate = DateTime.UtcNow                    
        //        };

        //        return RedirectToAction(nameof(Create), estimate);
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //[Route("Estimates/GetVehiclesAsync")]
        //public async Task<JsonResult> GetVehiclesAsync(int customerId)
        //{
        //    var vehicles = await _customerRepository.GetCustomerVehicleAsync(customerId);
        //    var json = Json(vehicles);
        //    return json;
        //}


    }
}

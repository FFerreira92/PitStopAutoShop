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
        private readonly IConverterHelper _converterHelper;

        public EstimatesController(IEstimateRepository estimateRepository, IServiceRepository serviceRepository,
                                   ICustomerRepository customerRepository, IVehicleRepository vehicleRepository,
                                   IUserHelper userHelper, IFlashMessage flashMessage, IConverterHelper converterHelper)
        {
            _estimateRepository = estimateRepository;
            _serviceRepository = serviceRepository;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
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

            double totalCost = 0;

            foreach (var item in listmodel)
            {
                totalCost += item.ValueWithDiscount;
            }
            
            ViewData["TotalCost"] = totalCost.ToString("C2");

            return View(listmodel);
           
        }
      

        public async Task<IActionResult> AddService(int? id, bool isEdit)
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
                IsEdit = isEdit,
                EstimateId = estimateDetailTemp.EstimateId,
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddService(AddServiceToEstimateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _serviceRepository.AddServiceToEstimateAsync(model, this.User.Identity.Name);
                if (!model.IsEdit)
                {
                    return RedirectToAction("Create", new { id = model.VehicleId });
                }
                else
                {
                    return RedirectToAction("Edit", new { id = model.VehicleId, isNew = false });
                }
                
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

        public async Task<IActionResult> Increase(int? id, bool isEdit)
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
            

            if(!isEdit)
            {
                return RedirectToAction("Create", new { id = estimateDetailTemp.VehicleId });
            }
            else
            {
                return RedirectToAction("Edit", new { id = estimateDetailTemp.VehicleId , isNew = false});
            }
            
        }

        public async Task<IActionResult> Decrease(int? id, bool isEdit)
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

            if (!isEdit)
            {
                return RedirectToAction("Create", new { id = estimateDetailTemp.VehicleId });
            }
            else
            {
                return RedirectToAction("Edit", new { id = estimateDetailTemp.VehicleId, isNew = false });
            }
        }

        public async Task<IActionResult> DeleteItem(int? id, bool isEdit)
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

            if (!isEdit)
            {
                return RedirectToAction("Create", new { id = vehicleid });
            }
            else
            {
                return RedirectToAction("Edit", new { id = vehicleid, isNew = false });
            }
        }


        public async Task<IActionResult> ConfirmEstimate(int? id,bool isEdit,string faultDescription)
        {
            if(id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(id.Value);

            if(vehicle == null)
            {
                return NotFound();
            }

            var estimateDetailTemps = await _estimateRepository.GetDetailTempsAsync(vehicle.Id, vehicle.CustomerId);
            
            if(estimateDetailTemps != null)
            {
                Response response;

                if (!isEdit)
                {
                   response = await _estimateRepository.ConfirmEstimateAsync(this.User.Identity.Name, vehicle.CustomerId, vehicle.Id,faultDescription);
                   if (response.IsSuccess == true)
                   {
                       return RedirectToAction("Index");
                   }
                }
                else
                {
                    response = await _estimateRepository.UpdateEstimateAsync(this.User.Identity.Name, vehicle.CustomerId, vehicle.Id,faultDescription);
                    if(response.IsSuccess == true && response.Results == false)
                    {
                        return RedirectToAction("Index");
                    }
                    if(response.IsSuccess == true && response.Results == true)
                    {
                        return RedirectToAction("Index", "WorkOrders");
                    }
                }              

                if (!isEdit)
                {
                    return RedirectToAction("Create", new { id = vehicle.Id });
                }
                else
                {
                    return RedirectToAction("Edit", new { id = vehicle.Id, isNew = false });
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }         
        }


        public async Task<IActionResult> Edit(int? id, bool isNew)
        {
            if(id == null)
            {
                return NotFound();
            }
    
            if (isNew)
            {
                var estimate = await _estimateRepository.GetEstimateWithDetailsByIdAsync(id.Value);

                if (estimate == null)
                {
                    return NotFound();
                }

                //deletes estimateDetailTemps if any already exists for the car and vehicle. Case User leaves the page while already editing
                //a estimate, if he goes back to edit again the temps are deleted and nothing unwanted is saved.
                var deletedTemps = await _estimateRepository.DeleteEstimateDetailTempsAsync(estimate.Vehicle.Id, estimate.Customer.Id);

                var estimateDetailsTemps = await _converterHelper.ToEstimateDetailTemps(estimate.Services, User.Identity.Name);

                foreach(var item in estimateDetailsTemps)
                {
                    item.EstimateId = estimate.Id;
                }

                try
                {
                    await _estimateRepository.CreateEstimatesDetailsTemps(estimateDetailsTemps);
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error editing the estimate! " + ex.InnerException);
                    return RedirectToAction(nameof(Index));
                }

                double totalCost = 0;

                foreach (var item in estimateDetailsTemps)
                {
                    totalCost += item.ValueWithDiscount;
                }

               
                ViewData["faultDescription"] = estimate.FaultDescription;
                
                ViewData["TotalCost"] = totalCost.ToString("C2");

                return View(estimateDetailsTemps);
            }
            else
            {

                //if action is called when editing the estimate(increase/decrease/delete Item actions), isNew boolean is passed as false and 
                //the value from id passed is the vehicle Id.

                var vehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(id.Value);

                if (vehicle == null)
                {
                    return NotFound();
                }

                var userDetailTemps = await _estimateRepository.GetEstimateDetailTempWithVehicleIdAsync(this.User.Identity.Name, vehicle);

                var listmodel = await _estimateRepository.GetDetailTempsAsync(userDetailTemps.VehicleId, userDetailTemps.CustomerId);

                var estimate = await _estimateRepository.GetEstimateWithDetailsByIdAsync(listmodel.FirstOrDefault().EstimateId);

                double totalCost = 0;

                foreach (var item in listmodel)
                {
                    totalCost += item.ValueWithDiscount;
                }

               
                ViewData["faultDescription"] = estimate.FaultDescription;               
                ViewData["TotalCost"] = totalCost.ToString("C2");

                return View(listmodel);
            }        

        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var estimate = await _estimateRepository.GetEstimateWithDetailsByIdAsync(id.Value);

            if (estimate == null)
            {
                return NotFound();
            }

            return View(estimate);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estimate = await _estimateRepository.GetByIdAsync(id.Value);

            if (estimate == null)
            {
                return NotFound();
            }

            try
            {
                var success = await _estimateRepository.DeleteEstimateDetailsAsync(estimate.Id);
                if(success > 0)
                {
                    await _estimateRepository.DeleteAsync(estimate);
                    _flashMessage.Confirmation("Estimate deleted with success!");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _flashMessage.Danger("There was an error deleting the estimate, probably the estimate is open elsewhere. ");
                    return RedirectToAction(nameof(Index));
                }               
            }
            catch (Exception ex)
            {
                _flashMessage.Danger("There was an error deleting the estimate, probably the estimate is open elsewhere. " + ex.InnerException );

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Estimates/RemoveTemps")]
        public async Task<bool> RemoveTemps(int vehicleId, int customerId)
        {

            if(vehicleId == 0 || customerId == 0)
            {
                return false;
            }            

            var result = await _estimateRepository.DeleteEstimateDetailTempsAsync(vehicleId, customerId);

            if (result > 0)
                return true;
            else
                return false;
        }


    }
}

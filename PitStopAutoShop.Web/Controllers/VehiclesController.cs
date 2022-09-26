using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using Vereyon.Web;

namespace PitStopAutoShop.Web.Controllers
{
    [Authorize(Roles = "Admin, Technician, Receptionist")]
    public class VehiclesController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBrandRepository _brandRepository;
        private readonly IEstimateRepository _estimateRepository;
        private readonly IFlashMessage _flashMessage;

        public VehiclesController(IVehicleRepository vehicleRepository,
            ICustomerRepository customerRepository,
            IUserHelper userHelper,
            IBrandRepository brandRepository,
            IEstimateRepository estimateRepository,
            IFlashMessage flashMessage)
        {
            _vehicleRepository = vehicleRepository;
            _customerRepository = customerRepository;
            _userHelper = userHelper;
            _brandRepository = brandRepository;
            _estimateRepository = estimateRepository;
            _flashMessage = flashMessage;
        }

        // GET: Vehicles
        public IActionResult Index()
        {
            var vehicles = _vehicleRepository.GetAllWithCustomers();

            return View(vehicles);
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public async Task<IActionResult> Create(string email, bool isEstimate)
        {
            var customer = await _customerRepository.GetCustomerByEmailAsync(email);

            if(customer == null)
            {
                return NotFound();
            }

            var vehicleModel = new VehicleViewModel
            {
                Customer = customer,
                CustomerId = customer.Id,
                Brands = _brandRepository.GetComboBrands(),
                Models = _brandRepository.GetComboModels(0),
                IsEstimate = isEstimate
            };

            return View(vehicleModel);
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleViewModel vehicleModel)
        {
            var customer = await _customerRepository.GetByIdAsync(vehicleModel.CustomerId);

            if (customer == null)
            {
                return NotFound();
            }

            vehicleModel.Customer = customer;

            if (ModelState.IsValid)
            {

                if (!CheckIfVinIsNull(vehicleModel.VehicleIdentificationNumber))
                {
                    if (vehicleModel.VehicleIdentificationNumber.Length < 17)
                    {
                        ModelState.AddModelError(string.Empty, "The Vehicle Identification Number (VIN) must have 17 characters.");
                        vehicleModel.Brands = _brandRepository.GetComboBrands();
                        vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);                        
                        return View(vehicleModel);
                    }
                }

                var brand = await _brandRepository.GetBrandWithModelsAsync(vehicleModel.BrandId);
                var model = await _brandRepository.GetModelAsync(vehicleModel.ModelId);

                if (model == null || brand == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error creating the vehicle.");
                    vehicleModel.Brands = _brandRepository.GetComboBrands();
                    vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);                    
                    return View(vehicleModel);
                }

                var vehicle = new Vehicle
                {
                    Brand = brand,
                    Model = model,
                    DateOfConstruction = vehicleModel.DateOfConstruction,
                    PlateNumber = vehicleModel.PlateNumber,
                    VehicleIdentificationNumber = vehicleModel.VehicleIdentificationNumber,
                    Horsepower = vehicleModel.Horsepower,
                    CustomerId = customer.Id,
                };

                try
                {
                    await _vehicleRepository.CreateAsync(vehicle);

                    if (!vehicleModel.IsEstimate)
                    {
                        return RedirectToAction("Edit", "Customer", new { id = vehicle.CustomerId });
                    }
                    else
                    {
                        var addedVehicle = await _vehicleRepository.GetNewlyAddedVehicleAsync(customer.Id);

                        var estimateDetailTemp = new EstimateDetailTemp
                        {
                            CustomerId = customer.Id,
                            VehicleId = addedVehicle.Id,
                            User = await _userHelper.GetUserByEmailAsync(User.Identity.Name)
                        };

                        try
                        {
                            await _estimateRepository.CreateEstimateDetailTemp(estimateDetailTemp);
                            return RedirectToAction("Create","Estimates", new { id = estimateDetailTemp.VehicleId });
                        }
                        catch (Exception ex)
                        {
                            _flashMessage.Danger("There was an error creating the estimate. " + ex.InnerException.Message);
                            return RedirectToAction("Index","Customer");
                        }                        
                    }
                    
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }          
      
            }

            vehicleModel.Brands = _brandRepository.GetComboBrands();
            vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
            return View(vehicleModel);
        }

        //// GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var vehicleModel = new VehicleViewModel
            {
                BrandId = vehicle.Brand.Id,
                DateOfConstruction = vehicle.DateOfConstruction,
                ModelId = vehicle.Model.Id,
                VehicleId = vehicle.Id,
                CustomerId = vehicle.CustomerId,
                PlateNumber = vehicle.PlateNumber,
                VehicleIdentificationNumber = vehicle.VehicleIdentificationNumber,
                Horsepower = vehicle.Horsepower,
                Brands = _brandRepository.GetComboBrands(),
                Models = _brandRepository.GetComboModels(vehicle.Brand.Id)
            };

            return View(vehicleModel);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VehicleViewModel vehicleModel)
        {     
            if (ModelState.IsValid)
            {
                if(!CheckIfVinIsNull(vehicleModel.VehicleIdentificationNumber))
                {
                    if(vehicleModel.VehicleIdentificationNumber.Length < 17)
                    {
                        ModelState.AddModelError(string.Empty, "The Vehicle Identification Number (VIN) must have 17 characters.");
                        vehicleModel.Brands = _brandRepository.GetComboBrands();
                        vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
                        return View(vehicleModel);
                    }
                }

                var vehicle = await _vehicleRepository.GetByIdAsync(vehicleModel.VehicleId);
                var brand = await _brandRepository.GetByIdAsync(vehicleModel.BrandId);
                var model = await _brandRepository.GetModelAsync(vehicleModel.ModelId);

                if(model == null || brand == null || vehicle == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error editing the vehicle.");
                    vehicleModel.Brands = _brandRepository.GetComboBrands();
                    vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
                    return View(vehicleModel);
                }

                vehicle.PlateNumber = vehicleModel.PlateNumber;
                vehicle.VehicleIdentificationNumber = vehicleModel.VehicleIdentificationNumber;
                vehicle.Horsepower = vehicleModel.Horsepower;
                vehicle.Brand = brand;
                vehicle.CustomerId = vehicleModel.CustomerId;
                vehicle.Model = model;
                vehicle.DateOfConstruction = vehicleModel.DateOfConstruction;

                try
                {
                    await _vehicleRepository.UpdateAsync(vehicle);
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    vehicleModel.Brands = _brandRepository.GetComboBrands();
                    vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
                    return View(vehicleModel);
                }                
                
                return RedirectToAction(nameof(Index));
            }
            vehicleModel.Brands = _brandRepository.GetComboBrands();
            vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
            return View(vehicleModel);
        }

        private bool CheckIfVinIsNull(string vinNumber)
        {
           bool vinIsNull = false;

            if (string.IsNullOrEmpty(vinNumber))
            { 
                vinIsNull = true;
            }
            
            return vinIsNull;
        }
     

        // POST: Vehicles/Delete/5              
        public async Task<IActionResult> Delete(int? id)
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

            try
            {
                await _vehicleRepository.DeleteAsync(vehicle);                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException.Message);                
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [Route("Vehicles/GetModelsAsync")]
        public async Task<JsonResult> GetModelsAsync(int brandId)
        {
            if(brandId == 0)
            {
                return null;
            }
            var brand = await _brandRepository.GetBrandWithModelsAsync(brandId);
            var json = Json(brand.Models.OrderBy(m => m.Name));
            return json;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;

namespace PitStopAutoShop.Web.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBrandRepository _brandRepository;

        public VehiclesController(IVehicleRepository vehicleRepository,
            ICustomerRepository customerRepository,
            IUserHelper userHelper,
            IBrandRepository brandRepository)
        {
            _vehicleRepository = vehicleRepository;
            _customerRepository = customerRepository;
            _userHelper = userHelper;
            _brandRepository = brandRepository;
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
        public async Task<IActionResult> Create(string email)
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
                        return View(vehicleModel);
                    }
                }

                var brand = await _brandRepository.GetBrandWithModelsAsync(vehicleModel.BrandId);
                var model = await _brandRepository.GetModelAsync(vehicleModel.ModelId);

                if (model == null || brand == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error creating the vehicle.");
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
                    return RedirectToAction("Edit", "Customer", new { id = vehicle.CustomerId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }           
      
            }

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
                        return View(vehicleModel);
                    }
                }

                var vehicle = await _vehicleRepository.GetByIdAsync(vehicleModel.VehicleId);
                var brand = await _brandRepository.GetByIdAsync(vehicleModel.BrandId);
                var model = await _brandRepository.GetModelAsync(vehicleModel.ModelId);

                if(model == null || brand == null || vehicle == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error editing the vehicle.");
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
                    return View(vehicleModel);
                }                
                
                return RedirectToAction(nameof(Index));
            }
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
            var brand = await _brandRepository.GetBrandWithModelsAsync(brandId);
            var json = Json(brand.Models.OrderBy(m => m.Name));
            return json;
        }

    }
}

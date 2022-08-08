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

        public VehiclesController(IVehicleRepository vehicleRepository,
            ICustomerRepository customerRepository,
            IUserHelper userHelper)
        {
            _vehicleRepository = vehicleRepository;
            _customerRepository = customerRepository;
            _userHelper = userHelper;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
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

            var vehicle = _vehicleRepository.GetVehicleDetailsByIdAsync(id.Value);

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

            var vehicleModel = new AddVehicleViewModel
            {
                Customer = customer,
                CustomerId = customer.Id,
            };

            return View(vehicleModel);
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddVehicleViewModel vehicleModel)
        {
            if (ModelState.IsValid)
            {
                var customer = await _customerRepository.GetByIdAsync(vehicleModel.CustomerId);

                if(customer == null)
                {
                    return NotFound();
                }

                var vehicle = new Vehicle
                {
                    Brand = vehicleModel.Brand,
                    Model = vehicleModel.Model,
                    DateOfConstruction = vehicleModel.DateOfConstruction,
                    PlateNumber = vehicleModel.PlateNumber,
                    VehicleIdentificationNumber = vehicleModel.VehicleIdentificationNumber,
                    Horsepower = vehicleModel.Horsepower,
                    CustomerId = customer.Id,
                };

                await _vehicleRepository.CreateAsync(vehicle);
                

                return RedirectToAction("Edit","Customer",new { id = vehicle.CustomerId });
            }
            return View(vehicleModel);
        }

        //// GET: Vehicles/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var vehicle = await _context.Vehicles.FindAsync(id);
        //    if (vehicle == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(vehicle);
        //}

        //// POST: Vehicles/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,DateOfConstruction,PlateNumber,VehicleIdentificationNumber,Horsepower")] Vehicle vehicle)
        //{
        //    if (id != vehicle.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(vehicle);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!VehicleExists(vehicle.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(vehicle);
        //}

        //// GET: Vehicles/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var vehicle = await _context.Vehicles
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (vehicle == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(vehicle);
        //}

        //// POST: Vehicles/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var vehicle = await _context.Vehicles.FindAsync(id);
        //    _context.Vehicles.Remove(vehicle);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool VehicleExists(int id)
        //{
        //    return _context.Vehicles.Any(e => e.Id == id);
        //}
    }
}

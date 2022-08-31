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
    
    public class AppointmentController : Controller
    {
        private readonly IFlashMessage _flashMessage;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEstimateRepository _estimateRepository;
        private readonly IUserHelper _userHelper;

        public AppointmentController(IFlashMessage flashMessage,IAppointmentRepository appointmentRepository
                                    ,IEmployeeRepository employeeRepository, ICustomerRepository customerRepository
                                    ,IEstimateRepository estimateRepository, IUserHelper userHelper)
        {
            _flashMessage = flashMessage;
            _appointmentRepository = appointmentRepository;
            _employeeRepository = employeeRepository;
            _customerRepository = customerRepository;
            _estimateRepository = estimateRepository;
            _userHelper = userHelper;
        }
        
        public IActionResult Index()
        {            
            ViewData["Events"] = _appointmentRepository.GetAllEvents();

            return View();
        }

        public async  Task<IActionResult> AddAppointment(int? id)
        {

            if(id == null)
            {
                return NotFound();
            }

            var estimateToAppoint = await _estimateRepository.GetEstimateWithDetailsByIdAsync(id.Value);

            if(estimateToAppoint == null)
            {
                return NotFound();
            }

            if(estimateToAppoint.HasAppointment != true)
            {
                var model = new AppointmentViewModel
                {
                    Technicians = _employeeRepository.GetComboTechnicians(),
                    CustomerId = estimateToAppoint.Customer.Id,
                    VehicleId = estimateToAppoint.Vehicle.Id,
                    EstimateId = estimateToAppoint.Id,
                    Customer = estimateToAppoint.Customer,
                    Vehicle = estimateToAppoint.Vehicle,
                };
                
                ViewData["Events"] = _appointmentRepository.GetAllEvents();

                return View(model);
            }

            _flashMessage.Warning("The estimate that you are selecting already has an appointment made!");
            return RedirectToAction("Index","Estimates");            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAppointment(AppointmentViewModel model)
        {
            var estimate = await _estimateRepository.GetEstimateWithDetailsByIdAsync(model.EstimateId);

            if (ModelState.IsValid)
            {

                var appointment = new Appointment
                {
                    Observations = model.Observations,
                    Mechanic = await _employeeRepository.GetEmployeeByIdAsync(model.EmployeeId),
                    Customer = estimate.Customer,
                    Vehicle = estimate.Vehicle,
                    Estimate = estimate,
                    CreatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                    UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                    AppointmentStartDate = model.AppointmentStartDate,
                    AppointmentEndDate = model.AppointmentEndDate,
                    CreatedDate = model.CreatedDate,
                    UpdatedDate = model.UpdatedDate,
                };              


                try
                {
                    await _appointmentRepository.CreateAsync(appointment);
                    estimate.HasAppointment = true;
                    await _estimateRepository.UpdateAsync(estimate);
                    _flashMessage.Confirmation("The appointment was created with success!");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was a problem creating the appointment! "+ ex.InnerException);
                    model.Customer = estimate.Customer;
                    model.Vehicle = estimate.Vehicle;
                    model.Technicians = _employeeRepository.GetComboTechnicians();
                    return View(model);
                }                
            }

            
            model.Customer = estimate.Customer;
            model.Vehicle = estimate.Vehicle;
            model.Technicians = _employeeRepository.GetComboTechnicians();
            return View(model);
        }                
        
    }
}

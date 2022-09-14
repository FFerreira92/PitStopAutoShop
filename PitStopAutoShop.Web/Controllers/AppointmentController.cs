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
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;

        public AppointmentController(IFlashMessage flashMessage,IAppointmentRepository appointmentRepository
                                    ,IEmployeeRepository employeeRepository, ICustomerRepository customerRepository
                                    ,IEstimateRepository estimateRepository, IUserHelper userHelper
                                    ,IConverterHelper converterHelper, IMailHelper mailHelper)
        {
            _flashMessage = flashMessage;
            _appointmentRepository = appointmentRepository;
            _employeeRepository = employeeRepository;
            _customerRepository = customerRepository;
            _estimateRepository = estimateRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
        }
        
        public IActionResult Index()
        {
            //adiciona um refresh rate à ação index do controlador
            
            Response.Headers.Add("Refresh", "60");
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
            
            string servicesString = "";
            foreach(var item in estimate.Services)
            {
                if(item.Service != null)
                {
                    servicesString += item.Service.Name + "<br>";
                }                
            }

            if (ModelState.IsValid)
            {
                string obs = "";

                if (string.IsNullOrEmpty(model.Observations))
                {
                    obs = "No observations added.";
                }
                else
                {
                    obs = model.Observations;
                }

                var appointment = new Appointment
                {                    
                    Observations = obs,
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
                    AppointmentServicesDetails = servicesString,
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

            ViewData["Events"] = _appointmentRepository.GetAllEvents();
            model.Customer = estimate.Customer;
            model.Vehicle = estimate.Vehicle;
            model.Technicians = _employeeRepository.GetComboTechnicians();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id.Value);

            if(appointment == null)
            {
                return NotFound();
            }

            //if(appointment.AppointmentStartDate < DateTime.UtcNow)
            //{
            //    _flashMessage.Warning("It is not possible to edit an appointment from the past.");
            //    return RedirectToAction(nameof(Index));
            //}

            var model = _converterHelper.ToAppointmentViewModel(appointment, false);

            if(model == null)
            {
                return NotFound();
            }
            
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentViewModel model)
        {
            //Pt- Devido à restrição imposta para que as marcações sejam todas procedidas para depois da data e hora atual, não permitia editar marcações do passado apesar de
            //dados se encontrarem preenchidos no modelo.
            //En - Due to the restriction imposed so that the appointments are all proceeded after the current date and time,
            //it did not allow editing of appointments from the past despite the data being filled in the model.

            if (ModelState.ErrorCount < 2 && !string.IsNullOrEmpty(model.AppointmentStartDate.ToString()))
            {
                var estimate = await _estimateRepository.GetEstimateWithDetailsByIdAsync(model.EstimateId);

                string obs = "";

                if (string.IsNullOrEmpty(model.Observations))
                {
                    obs = "No observations added.";
                }
                else
                {
                    obs = model.Observations;
                }

                var appointment = await _appointmentRepository.GetAppointmentByIdAsync(model.Id);

                if(appointment == null)
                {
                    _flashMessage.Danger("There was an error updating the appointment");
                    return RedirectToAction(nameof(Index));
                }

                appointment.Observations = obs;
                appointment.UpdatedDate = DateTime.Now;
                appointment.Mechanic = await _employeeRepository.GetEmployeeByIdAsync(model.EmployeeId);
                appointment.UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                try
                {
                    await _appointmentRepository.UpdateAsync(appointment);
                    _flashMessage.Confirmation("The appointment was updated with success!");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error updating the appointment "+ ex.Message);
                    model = _converterHelper.ToAppointmentViewModel(appointment, false);
                    return View(model);
                }
            }         

            _flashMessage.Danger("Failed to update the appointment.");
            return RedirectToAction(nameof(Index));
                      
        }

        [HttpPost]
        [Route("Appointment/EventResize")]
        public async Task<bool> EventResize(int eventId, string startTime,string endTime)
        {
            if(eventId == 0 || string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
            {
                return false;
            }

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(eventId);

            if(appointment == null)
            {
                return false;
            }

            var startDateTime = Convert.ToDateTime(startTime).ToUniversalTime();
            var endDateTime = Convert.ToDateTime(endTime).ToUniversalTime();

            appointment.AppointmentEndDate = endDateTime;
            appointment.AppointmentStartDate = startDateTime;
            appointment.UpdatedDate = DateTime.Now;
            appointment.UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            try
            {
                await _appointmentRepository.UpdateAsync(appointment);
                return true;
            }
            catch (Exception)
            {
                return false;
            }           
        }

        
        public async Task<IActionResult> Cancel(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id.Value);

            if(appointment == null)
            {
                return NotFound();
            }

            if(appointment.AsAttended == true)
            {
                _flashMessage.Warning($"It is not possible to cancel the appointment. The appointment already has a work order going or done.");
                return RedirectToAction(nameof(Index));
            }

            var customerFullName = appointment.Customer.FullName;
            var estimate = appointment.Estimate;
            var email = appointment.Customer.Email;
            var appointmentDate = appointment.AppointmentStartDate.ToString("dd/MM/yyyy");
            var appointmentHour = appointment.AppointmentStartDate.ToString("HH:mm");
            var vehicle = appointment.Vehicle;

            try
            {               
                await _appointmentRepository.DeleteAsync(appointment);
                
                estimate.HasAppointment = false;
                await _estimateRepository.UpdateAsync(estimate);

                await _mailHelper.SendEmail(email, "Appointment cancelation Pitstop Autoshop Lisbon", $"<h1>Appointment Cancelation</h1>" +
                    $" Mr/Mrs {customerFullName},<br>We send you this email to inform you that the appointment you had with our services for {appointmentDate} " +
                    $"at {appointmentHour} with your {vehicle.Brand.Name} {vehicle.Model.Name} ({vehicle.PlateNumber}) was canceled.<br> For more information, please contact us by either replying to this email or" +
                    $" to our contact number: 216589564. <br>" +
                    $"<br>Thank you for your time and hope to see you soon! <br> Best regards, <br> Pitstop Autoshop Lisbon ", null);
                
                _flashMessage.Warning($"The appointment from {customerFullName} was canceled with success.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Warning($"There was a problem canceling the appointment. {ex.InnerException}.");
                return RedirectToAction(nameof(Index));  
            }

        }

    }
}

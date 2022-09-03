using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using System;
using System.Threading.Tasks;
using Vereyon.Web;

namespace PitStopAutoShop.Web.Controllers
{
    public class WorkOrdersController : Controller
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserHelper _userHelper;

        public WorkOrdersController(
            IWorkOrderRepository workOrderRepository,
            IFlashMessage flashMessage,
            IAppointmentRepository appointmentRepository,
            IUserHelper userHelper)
        {
            _workOrderRepository = workOrderRepository;
            _flashMessage = flashMessage;
            _appointmentRepository = appointmentRepository;
            _userHelper = userHelper;
        }

        public IActionResult Index()
        {
            var workOrders = _workOrderRepository.GetAllWorkOrders();

            return View(workOrders);
        }

        public async Task<IActionResult> Create(int? id)
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

            var workOrder = new WorkOrder
            {
                Appointment = appointment,
                CreatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                IsFinished = false,
                OrderDateStart = DateTime.UtcNow,               
            };

            try
            {
                await _workOrderRepository.CreateAsync(workOrder);
                _flashMessage.Info($"{appointment.Customer.FullName} work Order was successfuly created.");
                appointment.AsAttended = true;
                await _appointmentRepository.UpdateAsync(appointment);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Info($"There was an error creating {appointment.Customer.FullName} work order. {ex.InnerException}.");
                return RedirectToAction("Index","Appointment");
            }
        }


    }
}

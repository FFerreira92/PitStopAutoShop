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
        private readonly IEmployeeRepository _employeeRepository;

        public WorkOrdersController(
            IWorkOrderRepository workOrderRepository,
            IFlashMessage flashMessage,
            IAppointmentRepository appointmentRepository,
            IUserHelper userHelper,
            IEmployeeRepository employeeRepository)
        {
            _workOrderRepository = workOrderRepository;
            _flashMessage = flashMessage;
            _appointmentRepository = appointmentRepository;
            _userHelper = userHelper;
            _employeeRepository = employeeRepository;
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


        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(id.Value);

            if(workOrder == null)
            {
                return NotFound();
            }       
 
            return RedirectToAction("Edit","Estimates", new {id= workOrder.Appointment.Estimate.Id, isNew = true });
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(id.Value);

            if (workOrder == null)
            {
                return NotFound();
            }

            return View(workOrder);
        }

        public async Task<IActionResult> DeclareDone(int? id, string observations,int employeeId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(id.Value);

            if (workOrder == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);

            if(employee == null)
            {
                return NotFound();
            }

            workOrder.ServiceDoneBy = employee.User;
            workOrder.Observations = observations;
            workOrder.IsFinished = true;
            workOrder.awaitsReceipt = true;
            workOrder.OrderDateEnd = DateTime.UtcNow;

            try
            {
                await _workOrderRepository.UpdateAsync(workOrder);
                _flashMessage.Confirmation($"Work order nº{workOrder.Id} as been declared finished.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Warning($"There was a problem declaring work order as done.");
                return RedirectToAction(nameof(Index));
            }           
        }

        [HttpPost]
        [Route("WorkOrders/CheckValidEmployeeId")]
        public async Task<JsonResult> CheckValidEmployeeId(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            var json = Json(employee);
            return json;
        }

        [HttpPost]
        [Route("WorkOrders/GetWorkOrder")]
        public async Task<JsonResult> GetWorkOrder(int workOrderId)
        {
            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(workOrderId);
            var json = Json(workOrder);
            return json;
        }
    }
}

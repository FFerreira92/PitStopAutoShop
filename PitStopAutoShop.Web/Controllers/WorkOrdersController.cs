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
        private readonly IInvoiceRepository _invoiceRepository;

        public WorkOrdersController(
            IWorkOrderRepository workOrderRepository,
            IFlashMessage flashMessage,
            IAppointmentRepository appointmentRepository,
            IUserHelper userHelper,
            IEmployeeRepository employeeRepository,
            IInvoiceRepository invoiceRepository)
        {
            _workOrderRepository = workOrderRepository;
            _flashMessage = flashMessage;
            _appointmentRepository = appointmentRepository;
            _userHelper = userHelper;
            _employeeRepository = employeeRepository;
            _invoiceRepository = invoiceRepository;
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
                Status = "Opened",
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

            if(workOrder.IsFinished != true)
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);

                if (employee == null)
                {
                    return NotFound();
                }

                workOrder.ServiceDoneBy = employee.User;
                workOrder.Observations = observations;
                workOrder.IsFinished = true;
                workOrder.awaitsReceipt = true;
                workOrder.Status = "Done";

                try
                {
                    await _workOrderRepository.UpdateAsync(workOrder);
                    _flashMessage.Confirmation($"Work order nº{workOrder.Id} as been declared done.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Warning($"There was a problem declaring work order as done.");
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                return NotFound();
            }            
        }

        public async Task<IActionResult> Delete(int? id)
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

            if(workOrder.Status != "Opened")
            {
                _flashMessage.Warning("It is not possible to delete/Cancel a work order that has already been processed.");
                return RedirectToAction(nameof(Index));
            }

            workOrder.Appointment.AsAttended = false;

            try
            {
                await _appointmentRepository.UpdateAsync(workOrder.Appointment);
                await _workOrderRepository.DeleteAsync(workOrder);
                _flashMessage.Warning($"Work order nº{workOrder.Id} as been canceled.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Warning($"There was an error canceling the order. {ex.InnerException}");
                return RedirectToAction(nameof(Index));
            }          

        }

        public async Task<IActionResult> PrintInvoice(int? id)
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

            if(workOrder.Status == "Done" && workOrder.IsFinished == true)
            {
                var invoice = new Invoice
                {
                    CreatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                    Customer = workOrder.Appointment.Customer,
                    Estimate = workOrder.Appointment.Estimate,
                    Vehicle = workOrder.Appointment.Vehicle,
                    WorkOrder = workOrder,
                    InvoicDate = DateTime.UtcNow,
                    Value = Convert.ToDecimal(workOrder.Appointment.Estimate.ValueWithDiscount),
                };

                try
                {
                    await _invoiceRepository.CreateAsync(invoice);
                    workOrder.Status = "Closed";
                    workOrder.OrderDateEnd = DateTime.UtcNow;
                    workOrder.UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                    workOrder.awaitsReceipt = false;
                    await _workOrderRepository.UpdateAsync(workOrder);
                    _flashMessage.Confirmation("The invoice was created with success.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Confirmation($"There was a problem creating the invoice. {ex.InnerException}");
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                return NotFound();
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

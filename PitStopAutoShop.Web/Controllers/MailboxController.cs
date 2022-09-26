using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Vereyon.Web;

namespace PitStopAutoShop.Web.Controllers
{
    [Authorize(Roles = "Admin, Technician, Receptionist")]
    public class MailboxController : Controller
    {
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly IAppointmentRepository _appointmentRepository;

        public MailboxController(IMailHelper mailHelper, IFlashMessage flashMessage, IAppointmentRepository appointmentRepository)
        {
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
            _appointmentRepository = appointmentRepository;
        }

        public IActionResult Announcement()
        {

            var model = new AnnouncementViewModel
            {
                To = _mailHelper.Destinations()
            };      

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Announcement(AnnouncementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = "";

                if (model.Attachment != null)
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\tempData\" + model.Attachment.FileName);

                    using (var stream = System.IO.File.Create(path))
                    {
                        await model.Attachment.CopyToAsync(stream);
                    }
                }

                var response = await _mailHelper.SendAnnouncementAsync(model.ToId, model.Subject, model.Message, path);

                if (!string.IsNullOrEmpty(path))
                {
                    System.IO.File.Delete(path);
                }

                if(response.IsSuccess == true)
                {
                    _flashMessage.Confirmation("The annoucment was successfuly sent!");
                    return RedirectToAction(nameof(Announcement));
                }
                else
                {
                    _flashMessage.Warning("There was an error sending the announcement!");
                    model.To = _mailHelper.Destinations();
                    return View(model);
                }                
            }

            model.To = _mailHelper.Destinations();

            return View(model);
        }

        public async Task<IActionResult> InformCustomerAppointment()
        {
            int emailsSent = 0;
            var tommorowAppointments = await _appointmentRepository.GetTommorowAppointmentsAsync(DateTime.UtcNow.AddDays(1));          

            if(tommorowAppointments == null)
            {                
                return RedirectToAction("Index","Appointment");
            }

            foreach(var appointment in tommorowAppointments)
            {
                if(!appointment.IsInformed)
                {
                    var response = await _mailHelper.SendEmail(appointment.Customer.Email, "Appointment Confirmation Pitstop Autoshop Lisbon", $"<h1>Appointment Confirmation</h1>" +
                    $" Mr/Mrs {appointment.Customer.FullName},<br>We send you this email to remind you that you have an appointment with our services for tommorow ({DateTime.UtcNow.AddDays(1).ToString("dd/MM/yyyy")}) " +
                    $"at {appointment.AppointmentStartDate.ToString("HH:mm")} with your {appointment.Vehicle.Brand.Name} {appointment.Vehicle.Model.Name} ({appointment.Vehicle.PlateNumber}).<br>" +
                    $"<br>Thanks for your time and Hope to see you Tommorow! <br> Best regards, <br> Pitstop Autoshop Lisbon ", null);

                    if (!response.IsSuccess)
                    {
                        _flashMessage.Danger("There was an error sending the confirmation emails");
                        return RedirectToAction("Index", "Appointment");
                    }
                    
                    try
                    {
                        appointment.IsInformed = true;
                        await _appointmentRepository.UpdateAsync(appointment);
                        emailsSent++;
                    }
                    catch (Exception)
                    {
                        _flashMessage.Danger("There was an error sending the confirmation emails");
                        return RedirectToAction("Index", "Appointment");
                    }                    
                }                
            }
                       
            if(emailsSent == 0)
            {
                _flashMessage.Info("All the customers for tommorow were already informed.");                
            }
            else
            {
                _flashMessage.Confirmation("All the customers for tommorow were informed!");
            }

            return RedirectToAction("Index", "Appointment");
        }
    }
}

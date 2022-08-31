using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;

        public AppointmentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllAppointmentsAtLaterDates()
        {
            return _context.Appointments.Include(a => a.Mechanic)
                                        .Include(a => a.Vehicle)
                                        .Include(a => a.CreatedBy)
                                        .Include(a => a.UpdatedBy)
                                        .Include(a => a.Customer)
                                        .Include(a => a.Estimate)
                                        .Where(a => a.AppointmentStartDate >= DateTime.Today);
        }

        public string GetAllEvents()
        {
            var events = _context.Appointments
                .Include(a => a.Vehicle)
                .Include(a => a.Customer)
                .Include(a => a.Mechanic)
                .Select(e => new
            {
                id = e.Id,
                title = e.Vehicle.PlateNumber + " - Customer: " + e.Customer.FirstName + " " + e.Customer.LastName + " - Technician: " + e.Mechanic.FirstName + " " + e.Mechanic.LastName,
                start = e.AppointmentStartDate.ToString(),
                end = e.AppointmentEndDate.ToString(),
                color = e.Mechanic.Color,                              
            }).ToList();

            return System.Text.Json.JsonSerializer.Serialize(events);

            //return new JsonResult(events);
        }
        
    }
}

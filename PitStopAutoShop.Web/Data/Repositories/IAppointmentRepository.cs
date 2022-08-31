using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        IQueryable GetAllAppointmentsAtLaterDates();
        string GetAllEvents();
    }
}

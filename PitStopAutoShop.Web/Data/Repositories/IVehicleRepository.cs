using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        IQueryable GetAllWithCustomers();     

        IEnumerable<SelectListItem> GetComboVehicles(int customerId);

        Task<Vehicle> GetVehicleDetailsByIdAsync(int id);

        IQueryable GetCustomerVehiclesAsync(int customerId);

        Task<Vehicle> GetNewlyAddedVehicleAsync(int id);
    }
}

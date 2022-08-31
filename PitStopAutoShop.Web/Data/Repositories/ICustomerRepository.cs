using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        IQueryable GetAllWithUsers();

        Task<Customer> GetCustomerWithUserByIdAsync(int customerId);

        Task<bool> CheckIfCustomerInBdByEmailAsync(string customerEmail);

        Task<Customer> GetCustomerByUserIdAsync(string userId);

        Task<Customer> GetCustomerByEmailAsync(string email);

        IEnumerable<SelectListItem> GetComboCustomers();        

        Task<List<Vehicle>> GetCustomerVehicleAsync(int customerId);
    }
}

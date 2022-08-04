using PitStopAutoShop.Web.Data.Entities;
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
    }
}

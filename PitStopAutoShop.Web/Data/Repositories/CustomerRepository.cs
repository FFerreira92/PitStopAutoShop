using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly DataContext _context;

        public CustomerRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CheckIfCustomerInBdByEmailAsync(string customerEmail)
        {
            bool inBd = false;

            var customer = await _context.Customers.Where(ctm => ctm.Email == customerEmail).FirstOrDefaultAsync();

            if(customer != null)
            {
                inBd = true;
            }

            return inBd;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Customers.Include(u => u.User).OrderBy(o => o.FirstName);
        }

        public async Task<Customer> GetCustomerWithUserByIdAsync(int customerId)
        {
            var customer = await _context.Customers.Include(u => u.User).FirstOrDefaultAsync(u => u.Id == customerId);

            return customer;
;
        }
    }
}

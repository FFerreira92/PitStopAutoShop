using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System.Collections.Generic;
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
            return _context.Customers.Include(u => u.User)
                                     .Include(v => v.Vehicles)
                                     .OrderBy(o => o.FirstName);
        }

        public IEnumerable<SelectListItem> GetComboCustomers()
        {

            var list = _context.Customers.Select(b => new SelectListItem
            {
                Text = $"{b.FirstName} {b.LastName}",
                Value = b.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a Customer]",
                Value = "0"
            });

            return list;
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.Include(u => u.User)
                                           .Include(v => v.Vehicles)
                                           .Where(c => c.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Customer> GetCustomerByUserIdAsync(string userId)
        {
            return await _context.Customers.Include(v => v.Vehicles).Where(c => c.User.Id == userId).FirstOrDefaultAsync();
        }


        public async Task<Customer> GetCustomerWithUserByIdAsync(int customerId)
        {
            var customer = await _context.Customers.Include(u => u.User)
                                                   .Include(v => v.Vehicles)
                                                   .ThenInclude(v => v.Brand)
                                                   .ThenInclude(b => b.Models)
                                                   .FirstOrDefaultAsync(u => u.Id == customerId);

            return customer;
;
        }

        public async Task<List<Vehicle>> GetCustomerVehicleAsync(int customerId)
        {
            var customerVehicles = await _context.Vehicles.Where(p => p.CustomerId == customerId).ToListAsync();

            return customerVehicles;
            

        }
    }
}

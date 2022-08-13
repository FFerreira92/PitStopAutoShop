using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly DataContext _context;

        public VehicleRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithCustomers()
        {
            return _context.Vehicles.Include(v => v.Customer)
                                    .Include(m => m.Model)
                                    .Include(b => b.Brand)
                                    .OrderBy(v => v.Brand.Name);
        }

        public IEnumerable<SelectListItem> GetComboVehicles(int customerId)
        {
            var customer = _context.Customers.Find(customerId);
            var list = new List<SelectListItem>();

            if(customer != null)
            {
                list = _context.Vehicles.Where(v => v.CustomerId == customer.Id).Select(l => new SelectListItem
                {
                    Text = $"{l.PlateNumber}:{l.Brand.Name} - {l.Model.Name}",
                    Value = l.Id.ToString()
                }).OrderBy(p => p.Value).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "[Select a Vehicle]",
                    Value = "0"
                });
            }


            

            return list;
        }

        public IQueryable GetCustomerVehiclesAsync(int customerId)
        {
            return _context.Vehicles.Include(v => v.Customer)
                                          .Include(b => b.Brand)
                                          .Include(m => m.Model)
                                          .Where(v => v.CustomerId == customerId);


        }

        public async Task<Vehicle> GetVehicleDetailsByIdAsync(int id)
        {
            return await _context.Vehicles.Include(v => v.Customer)
                                          .Include(b => b.Brand)
                                          .Include(m => m.Model)
                                          .Where(v => v.Id == id).FirstOrDefaultAsync();
        }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class MechanicRepository : GenericRepository<Mechanic>, IMechanicRepository
    {
        private readonly DataContext _context;

        public MechanicRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Mechanics.Include(m => m.User);
        }

        public IEnumerable<SelectListItem> GetComboMechanics()
        {
            var list = _context.Mechanics.Select(m => new SelectListItem
            {
                Text = m.User.FullName,
                Value = m.Id.ToString()

            }).OrderBy(m => m.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Select a Mechanic",
                Value= "0"
            });

            return list;

        }

        public async Task<Mechanic> GetMechanicByIdAsync(int mechanicId)
        {
            var mechanic = await _context.Mechanics.Include(m => m.User).Where(m => m.Id == mechanicId).FirstOrDefaultAsync();

            return mechanic;
        }
    }
}

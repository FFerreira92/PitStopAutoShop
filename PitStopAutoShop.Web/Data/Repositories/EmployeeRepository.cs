using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly DataContext _context;

        public EmployeeRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Response> CheckIfEmployeeExistsAsync(User user)
        {
            var employee = await _context.Employees.FindAsync(user);

            if(employee == null)
            {
                return new Response { IsSuccess = false };
            }
            else
            {
                return new Response { IsSuccess = true };
            }
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Employees.Include(e => e.User)
                                     .Include(e => e.Role)
                                     .Include(e => e.Specialty);
        }

        public async Task<Employee> GetByEmailAsync(string email)
        {
            var employee = await _context.Employees.Include(e => e.User)
                                                   .Include(e => e.Role)
                                                   .Include(e => e.Specialty)
                                                   .Where(e => e.Email == email).FirstOrDefaultAsync();

            return employee;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees.Include(e => e.User)
                                                   .Include(e => e.Role)
                                                   .Include(e => e.Specialty)
                                                   .Where(m => m.Id == employeeId).FirstOrDefaultAsync();

            return employee;
        }
    }
}

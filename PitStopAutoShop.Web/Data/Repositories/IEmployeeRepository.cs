using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Helpers;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        IQueryable GetAllWithUsers();    

        Task<Employee> GetEmployeeByIdAsync(int employeeId);

        Task<Response> CheckIfEmployeeExistsAsync(User user);

        Task<Employee> GetByEmailAsync(string email);

        IEnumerable<SelectListItem> GetComboTechnicians();
        Task<List<Employee>> GetTechniciansEmployeesAsync();
    }
}

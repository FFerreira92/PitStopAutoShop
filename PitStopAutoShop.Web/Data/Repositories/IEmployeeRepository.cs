using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;


namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        IQueryable GetAllWithUsers();    

        Task<Employee> GetEmployeeByIdAsync(int employeeId);

    }
}

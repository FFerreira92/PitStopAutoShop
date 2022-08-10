using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IEmployeesRolesRepository : IGenericRepository<Role>
    {
        IQueryable GetRolesWithSpecialties();

        Task<Role> GetRoleWithSpecialtiesAsync(int id);

        Task<Specialty> GetSpecialtyAsync(int id);

        Task AddSpecialtyAsync(SpecialtyViewModel model);

        Task<int> UpdateSpecialtyAsync(Specialty model);

        Task<int> DeleteSpecialtyAsync(Specialty model);

        IEnumerable<SelectListItem> GetComboRoles();

        IEnumerable<SelectListItem> GetComboSpecialty(int roleId);

        Task<Role> GetRoleAsync(Specialty model);

        Task<int> GetRoleIdWithSpecialtyAsync(int specialtyId);

    }
}

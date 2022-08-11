using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Helpers
{
    public interface IConverterHelper
    {
        Role toRole(RoleViewModel model, bool isNew);

        RoleViewModel toRoleViewModel(Role role);

        Task<Employee> ToEmployee(EmployeeViewModel model,User user,bool isNew);

        EmployeeViewModel ToEmployeeViewModel(Employee employee,bool isNew);
    }
}

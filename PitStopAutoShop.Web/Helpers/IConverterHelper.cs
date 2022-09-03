using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Helpers
{
    public interface IConverterHelper
    {
        Role toRole(RoleViewModel model, bool isNew);

        RoleViewModel toRoleViewModel(Role role);

        Task<Employee> ToEmployee(EmployeeViewModel model,User user,bool isNew);

        EmployeeViewModel ToEmployeeViewModel(Employee employee,bool isNew);

        ServiceViewModel ToServiceViewModel(Service service);

        Service ToService(ServiceViewModel model, bool isNew);

        AppointmentViewModel ToAppointmentViewModel(Appointment appointment,bool isNew);

        Task<List<EstimateDetailTemp>> ToEstimateDetailTemps(IEnumerable<EstimateDetail> estimateDetails,string username);

        
    }
}

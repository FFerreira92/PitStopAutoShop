using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IWorkOrderRepository : IGenericRepository<WorkOrder>
    {
        IQueryable GetAllWorkOrders();

        Task<WorkOrder> GetWorkOrderByIdAsync(int id);

        Task<int> GetOpenedWorkOrdersAsync();
        Task<WorkOrder> GetWorkOrderByAppointmentIdAsync(int appointmentId);
        
        Task<List<WorkOrderChartDataModel>> GetWorkOrdersChartAsync(int month);
        Task<int> GetActiveWorkOrdersNumber();
    }
}

using PitStopAutoShop.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IWorkOrderRepository : IGenericRepository<WorkOrder>
    {
        IQueryable GetAllWorkOrders();

        Task<WorkOrder> GetWorkOrderByIdAsync(int id);

        Task<int> GetOpenedWorkOrdersAsync();
    }
}

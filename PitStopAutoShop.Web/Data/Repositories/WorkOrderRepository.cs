using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class WorkOrderRepository : GenericRepository<WorkOrder>, IWorkOrderRepository
    {
        private readonly DataContext _context;

        public WorkOrderRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWorkOrders()
        {
            return _context.WorkOrders
                .Include(wo => wo.ServiceDoneBy)
                .Include(wo => wo.UpdatedBy)
                .Include(wo => wo.CreatedBy)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Customer)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Mechanic)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Estimate)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Brand)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Model);
        }

        public async Task<int> GetOpenedWorkOrdersAsync()
        {
            var workOrders = _context.WorkOrders.Where(wo => wo.Status == "Opened");
            return workOrders.Count();
        }

        public async Task<WorkOrder> GetWorkOrderByIdAsync(int id)
        {
            return await _context.WorkOrders                
                .Include(wo => wo.ServiceDoneBy)
                .Include(wo => wo.UpdatedBy)
                .Include(wo => wo.CreatedBy)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Customer)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Mechanic)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Estimate)
                        .ThenInclude(e => e.Services)
                            .ThenInclude(es => es.Service)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Brand)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Model)
                .Where(wo => wo.Id == id).FirstAsync();
        }
    }
}

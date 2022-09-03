using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System.Linq;

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
    }
}

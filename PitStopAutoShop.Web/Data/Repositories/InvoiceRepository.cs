using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly DataContext _context;

        public InvoiceRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllInvoices()
        {
            return _context.Invoices
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.ServiceDoneBy)                  
                .Include(i => i.Customer)
                .Include(i => i.CreatedBy)
                .Include(i => i.Estimate)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Model);
        }

        public async Task<Invoice> GetInvoiceDetailsByIdAsync(int id)
        {
            return await _context.Invoices                                  
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.ServiceDoneBy)
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.Appointment)                            
                .Include(i => i.Customer)
                .Include(i => i.CreatedBy)
                .Include(i => i.Estimate)
                    .ThenInclude(e => e.Services)
                            .ThenInclude(es => es.Service)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Model)
                .Where(i => i.Id == id).FirstAsync();
        }

        public async Task<Invoice> GetRecentCreatedInvoiceAsync(int workOrderId)
        {
            return await _context.Invoices
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.ServiceDoneBy)
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.Appointment)
                .Include(i => i.Customer)
                .Include(i => i.CreatedBy)
                .Include(i => i.Estimate)
                    .ThenInclude(e => e.Services)
                            .ThenInclude(es => es.Service)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Model)
                .Where(i => i.WorkOrder.Id == workOrderId).FirstAsync();
        }
    }
}

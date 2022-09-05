using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System.Linq;

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
                .Include(i => i.WorkOrder)
                .Include(i => i.Customer)
                .Include(i => i.CreatedBy)
                .Include(i => i.Estimate)
                .Include(i => i.Vehicle);               
        }
    }
}

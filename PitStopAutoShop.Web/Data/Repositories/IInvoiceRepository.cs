using PitStopAutoShop.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        IQueryable GetAllInvoices();
        Task<Invoice> GetInvoiceDetailsByIdAsync(int id);
        Task<Invoice> GetRecentCreatedInvoiceAsync(int workOrderId);
    }
}

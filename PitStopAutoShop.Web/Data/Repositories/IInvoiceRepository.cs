using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        IQueryable GetAllInvoices();
        Task<Invoice> GetInvoiceDetailsByIdAsync(int id);
        Task<Invoice> GetRecentCreatedInvoiceAsync(int workOrderId);

        Task<List<SalesChartDataModel>> GetMonthlySales(int month);
        Task<List<SalesChartDataModel>> GetYearSalesByMonthAsync(int year);
        Task<List<Invoice>> GetUserInvoicesAsync(int customerId);
    }
}

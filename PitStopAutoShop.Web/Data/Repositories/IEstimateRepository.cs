using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IEstimateRepository : IGenericRepository<Estimate>
    {
              

        IQueryable GetAllEstimates();

        
    }
}

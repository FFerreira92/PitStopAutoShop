using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IEstimateRepository : IGenericRepository<Estimate>
    {             

        IQueryable GetAllEstimates();
        Task<IQueryable<EstimateDetailTemp>> GetDetailTempsAsync(int vehicleId, int customerId);
        Task CreateEstimateDetailTemp(EstimateDetailTemp estimateDetailTemp);
        Task<EstimateDetailTemp> GetEstimateDetailTempAsync(string userName);
        Task ModifyEstimateDetailTempQuantityAsync(int id, double quantity);
        Task DeleteDetailTempAsync(int id);
        Task<Response> ConfirmEstimateAsync(string username, int customerId,int vehicleId,string faultdescription);
        Task<EstimateDetailTemp> GetEstimateDetailTempWithVehicleIdAsync(string name, Vehicle vehicle);
        Task<EstimateDetailTemp> GetEstimateDetailTempByIdAsync(int id);
        Task<Estimate> GetEstimateWithDetailsByIdAsync(int value);
        Task<int> DeleteEstimateDetailsAsync(int id);
        Task CreateEstimatesDetailsTemps(IEnumerable<EstimateDetailTemp> estimateDetailTemps);
        Task<int> DeleteEstimateDetailTempsAsync(int vehicleId, int customerId);
        Task<Response> UpdateEstimateAsync(string username, int customerId, int vehicleId, string faultdescription);
    }
}

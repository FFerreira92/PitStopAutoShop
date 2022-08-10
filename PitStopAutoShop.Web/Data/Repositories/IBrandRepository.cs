using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        IQueryable GetBrandsWithModels();

        Task<Brand> GetBrandWithModelsAsync(int id);

        Task<Model> GetModelAsync(int id);

        Task AddModelAsync(ModelViewModel model);

        Task<int> UpdateModelAsync(Model model);

        Task<int> DeleteModelAsync(Model model);

        IEnumerable<SelectListItem> GetComboBrands();

        IEnumerable<SelectListItem> GetComboModels(int brandId);

        Task<Brand> GetBrandAsync(Model model);

        Task<int> GetBrandIdWithVehicleModelAsync(int modelId);

    }
}

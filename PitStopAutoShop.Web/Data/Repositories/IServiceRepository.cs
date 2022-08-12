using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        public IEnumerable<SelectListItem> GetComboServices();

    }
}

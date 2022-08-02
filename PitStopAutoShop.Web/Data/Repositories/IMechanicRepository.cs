using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;


namespace PitStopAutoShop.Web.Data.Repositories
{
    public interface IMechanicRepository : IGenericRepository<Mechanic>
    {
        IQueryable GetAllWithUsers();

        IEnumerable<SelectListItem> GetComboMechanics();

        Task<Mechanic> GetMechanicByIdAsync(int mechanicId);

    }
}

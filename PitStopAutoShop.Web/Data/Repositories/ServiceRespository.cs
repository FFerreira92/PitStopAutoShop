using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class ServiceRespository : GenericRepository<Service>, IServiceRepository
    {
        private readonly DataContext _context;

        public ServiceRespository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboServices()
        {
            var list = _context.Services.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString(),
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a service]",
                Value = "0"
            });

            return list;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class EstimateRepository : GenericRepository<Estimate>, IEstimateRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public EstimateRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }


        public IQueryable GetAllEstimates()
        {
            return _context.Estimates.Include(e => e.Customer)
                                     .Include(e => e.Vehicle)
                                     .Include(e => e.CreatedBy)
                                     .OrderBy(d => d.EstimateDate);
        }

    }
}

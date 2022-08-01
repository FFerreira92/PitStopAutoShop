using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;

namespace PitStopAutoShop.Web.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Mechanic> Mechanics { get; set; }


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
                
        }

    }
}

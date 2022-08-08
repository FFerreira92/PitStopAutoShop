using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;

namespace PitStopAutoShop.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Mechanic> Mechanics { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
                
        }

    }
}

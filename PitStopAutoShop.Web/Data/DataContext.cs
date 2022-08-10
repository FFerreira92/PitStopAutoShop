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

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Model> Models { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Vehicle>().HasIndex(v => v.PlateNumber).IsUnique();

            builder.Entity<Customer>().HasIndex(c => c.Nif).IsUnique();
        }

    }
}

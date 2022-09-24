using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using System.Linq;
using System.Reflection.Emit;

namespace PitStopAutoShop.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> EmployeesRoles { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Service> Services { get; set; }    
        public DbSet<Estimate> Estimates { get; set; }
        public DbSet<EstimateDetail> EstimateDetails { get; set; }
        public DbSet<EstimateDetailTemp> EstimateDetailTemps { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {     
           base.OnModelCreating(builder);

           builder.Entity<Vehicle>().HasIndex(v => v.PlateNumber).IsUnique();

           builder.Entity<Customer>().HasIndex(c => c.Nif).IsUnique();

           builder.Entity<Role>().HasIndex(r => r.Name).IsUnique();

           builder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
        }

    }
}

using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CheckCreatedRoles();
            await AddUserAsync();
            await AddEmployeesRolesAsync();
            await AddEmployeesAsync();
            await AddBrandsAsync();
            await AddServicesAsync();

        }

        private async Task AddServicesAsync()
        {
            if (!_context.Services.Any())
            {
                _context.Services.Add(new Service
                {
                    Name = "Change tyres",
                    Description = "Using the latest technology and tools to work your tyres. We change the valve...",
                    Price = 7.40m,
                    Discount = 0m
                });

                _context.Services.Add(new Service
                {
                    Name = "Oil Change",
                    Description = "We use the oil you choose and the way you want!",
                    Price = 29.90m,
                    Discount = 0m
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddEmployeesRolesAsync()
        {
            if (!_context.EmployeesRoles.Any())
            {
                var specialties = new List<Specialty>();

                specialties.Add(new Specialty { Name = "Mechanic" });
                specialties.Add(new Specialty { Name = "Electrician" });
                specialties.Add(new Specialty { Name = "Painter" });

                _context.EmployeesRoles.Add(new Role
                {
                    Specialties = specialties,
                    Name = "Technician",
                    PermissionsName = "Technician"
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddBrandsAsync()
        {
            if (!_context.Brands.Any())
            {
                var models = new List<Model>();

                models.Add(new Model { Name = "Astra" });
                models.Add(new Model { Name = "Combo" });
                models.Add(new Model { Name = "Corsa" });
                models.Add(new Model { Name = "GrandLand" });
                models.Add(new Model { Name = "Insignia" });
                models.Add(new Model { Name = "Mokka" });
                models.Add(new Model { Name = "Zafira" });

                _context.Brands.Add(new Brand
                {
                    Models = models,
                    Name = "Opel"
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCreatedRoles()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Technician");
            await _userHelper.CheckRoleAsync("Receptionist");
            await _userHelper.CheckRoleAsync("Customer");
        }

        private async Task AddUserAsync()
        {
            var user = await _userHelper.GetUserByEmailAsync("f92ferreira@gmail.com");

            if(user == null)
            {
                user = new User
                {
                    FirstName = "Filipe",
                    LastName = "Ferreira",
                    Email = "f92ferreira@gmail.com",
                    UserName = "f92ferreira@gmail.com",
                    PhoneNumber = "925648979",
                    Address = "Avenida Fialho Gouveia"
                };

                await _userHelper.AddUserAsync(user, "123456");
            }

            var isInRole = await _userHelper.CheckUserInRoleAsync(user, "Admin");

            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            await _userHelper.ConfirmEmailAsync(user, token);

            await _context.SaveChangesAsync();
        }

        private async Task AddEmployeesAsync()
        {
            if (!_context.Employees.Any())
            {

                var employeeUser1 = new User
                {
                    FirstName = "Joaquim",
                    LastName = "Guedes",
                    Email = "joaquimguedes@yopmail.com",
                    UserName = "joaquimguedes@yopmail.com",
                    PhoneNumber = "965897956",
                    Address ="Rua do barco"
                };

                _context.Employees.Add(new Employee
                {
                    FirstName = "Joaquim",
                    LastName = "Guedes",
                    About = "Born in Lisbon, Joaquim Guedes started his electrician carrer in Bosch Car Service in Lisbon...",
                    User = employeeUser1,
                    Role = _context.EmployeesRoles.Where(r => r.Name == "Technician").FirstOrDefault(),
                    Specialty = _context.Specialties.Where(s => s.Name == "Electrician").FirstOrDefault(),
                    Email = employeeUser1.Email
                });

                await _userHelper.AddUserAsync(employeeUser1, "123456");                
                await _userHelper.AddUserToRoleAsync(employeeUser1, "Technician");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(employeeUser1);
                await _userHelper.ConfirmEmailAsync(employeeUser1, token);

                var employeeUser2 = new User
                {
                    FirstName = "Inacio",
                    LastName = "Torres",
                    Email = "inaciotorres@yopmail.com",
                    UserName = "inaciotorres@yopmail.com",
                    PhoneNumber = "965896425",
                    Address = "Rua da banana"
                };

                _context.Employees.Add(new Employee
                {
                    FirstName = "Inacio",
                    LastName = "Torres",                    
                    About = "Born in Setúbal, Inacio Torres studied mechatronics in ATEC and then joined PitStop Auto, with 8 years of working experience..",
                    User = employeeUser2,
                    Role = _context.EmployeesRoles.Where(r => r.Name == "Technician").FirstOrDefault(),
                    Specialty = _context.Specialties.Where(s => s.Name == "Mechanic").FirstOrDefault(),
                    Email = employeeUser2.Email
                });

                await _userHelper.AddUserAsync(employeeUser2, "123456");                
                await _userHelper.AddUserToRoleAsync(employeeUser2, "Technician");
                token = await _userHelper.GenerateEmailConfirmationTokenAsync(employeeUser2);
                await _userHelper.ConfirmEmailAsync(employeeUser2, token);

                await _context.SaveChangesAsync();

            }
        }
    }
}

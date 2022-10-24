using Microsoft.EntityFrameworkCore;
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
            await AddCustomerAsync();
            await AddCustomerVehiclesAsync();
            await AddEstimateAsync();
            await AddAppointmentAsync();
            await AddWorkOrderAsync();
            await AddInvoiceAsync();

        }

        private async Task AddInvoiceAsync()
        {
            if (!_context.Invoices.Any())
            {
                var workOrder = await _context.WorkOrders.FirstOrDefaultAsync();
                var mechanic = await _userHelper.GetUserByEmailAsync("inaciotorres@yopmail.com");
                var customer = await _context.Customers.Where(c => c.Email == "Goncalo@yopmail.com").FirstOrDefaultAsync();
                var receptionist = await _userHelper.GetUserByEmailAsync("Pedrorato@yopmail.com");
                var estimate = await _context.Estimates.FirstOrDefaultAsync();
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "23-TO-50").FirstOrDefaultAsync();

                workOrder.Status = "Closed";
                workOrder.Observations = "It is highly recommended for the customer to align direction next time he comes to the shop";
                workOrder.ServiceDoneBy = mechanic;
                workOrder.IsFinished = true;

                _context.WorkOrders.Update(workOrder);

                _context.Invoices.Add(new Invoice
                {
                     Customer = customer,
                     CreatedBy = receptionist,
                     Estimate = estimate,
                     InvoicDate = DateTime.UtcNow.AddDays(-3),
                     Vehicle = vehicle,
                     WorkOrder = workOrder,
                     Value = (decimal)estimate.ValueWithDiscount,                     
                });


                await _context.SaveChangesAsync();
            }
        }

        private async Task AddWorkOrderAsync()
        {

            if (!_context.WorkOrders.Any())
            {

                var appointment = await _context.Appointments.FirstOrDefaultAsync();
                var receptionist = await _userHelper.GetUserByEmailAsync("Pedrorato@yopmail.com");
                var mechanic = await _context.Employees.Where(m => m.Email == "inaciotorres@yopmail.com").FirstOrDefaultAsync();

                _context.WorkOrders.Add(new WorkOrder
                {
                    Appointment = appointment,
                    awaitsReceipt = true,
                    CreatedBy = receptionist,
                    IsFinished = false,
                    OrderDateStart = DateTime.UtcNow.AddDays(-3).AddHours(-2),
                    OrderDateEnd = DateTime.UtcNow.AddDays(-3),
                    Status = "Opened",              
                    UpdatedBy = receptionist,                    
                });

                appointment.AsAttended = true;
                _context.Appointments.Update(appointment);

                await _context.SaveChangesAsync();
            }

        }

        private async Task AddAppointmentAsync()
        {
            if (!_context.Appointments.Any())
            {
                var customer = await _context.Customers.Where(c => c.Email == "Goncalo@yopmail.com").FirstOrDefaultAsync();
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "23-TO-50").FirstOrDefaultAsync();
                var estimate = await _context.Estimates.FirstOrDefaultAsync();
                var mechanic = await _context.Employees.Where(m => m.Email == "inaciotorres@yopmail.com").FirstOrDefaultAsync();
                var receptionist = await _userHelper.GetUserByEmailAsync("Pedrorato@yopmail.com");

                _context.Appointments.Add(new Appointment
                {
                    AppointmentStartDate = DateTime.UtcNow.AddDays(-3).AddHours(-2),
                    AppointmentEndDate = DateTime.UtcNow.AddDays(-3),
                    AppointmentServicesDetails = estimate.FaultDescription,
                    AsAttended = false,
                    CreatedBy = receptionist,
                    CreatedDate = DateTime.UtcNow.AddDays(-5),
                    Customer = customer,
                    Estimate = estimate,
                    Mechanic = mechanic,
                    Vehicle = vehicle,
                    UpdatedBy = receptionist,
                    UpdatedDate = DateTime.UtcNow.AddDays(-5),
                    Observations = "Customer does not want to align the direction",
                });

                estimate.HasAppointment = true;
                _context.Estimates.Update(estimate);

                await _context.SaveChangesAsync();
            }

        }

        private async Task AddCustomerVehiclesAsync()
        {
            if (!_context.Vehicles.Any())
            {

                var brand = await _context.Brands.Where(b => b.Name == "Opel").FirstOrDefaultAsync();
                var model = await _context.Models.Where(m => m.Name == "Corsa").FirstOrDefaultAsync();
                var customer = await _context.Customers.Where(c => c.Email == "Goncalo@yopmail.com").FirstOrDefaultAsync();

                _context.Vehicles.Add( new Vehicle
                {
                    Brand = brand,
                    Model = model,
                    Customer = customer,
                    DateOfConstruction = DateTime.Now.AddYears(-5),
                    PlateNumber = "23-TO-50",
                    Horsepower = 90,
                    CustomerId = customer.Id,
                });                


                await _context.SaveChangesAsync();

            }
        }

        private async Task AddEstimateAsync()
        {
            if (!_context.Estimates.Any())
            {

                var customer = await _context.Customers.Where(c => c.Email == "Goncalo@yopmail.com").FirstOrDefaultAsync();
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "23-TO-50").FirstOrDefaultAsync();

                var estimate = new Estimate
                {
                    CreatedBy = await _userHelper.GetUserByEmailAsync("Pedrorato@yopmail.com"),
                    Customer = customer,
                    EstimateDate = DateTime.Now.AddDays(-5),
                    FaultDescription = "Blowed up Tyre, needs to replace front tyres.",
                    HasAppointment = false,
                    Vehicle = vehicle,                     
                };

                _context.Estimates.Add(estimate);

                await _context.SaveChangesAsync();

                var service = await _context.Services.Where(s => s.Name == "Change tyres").FirstOrDefaultAsync();
                var estimateResult = await _context.Estimates.FirstOrDefaultAsync();             

                var estimateDetail1 = new EstimateDetail
                {
                    CustomerId = customer.Id,
                    EstimateId = estimateResult.Id,
                    Service = service,
                    Quantity = 2,
                    Price = service.Price * 2,
                    VehicleId = vehicle.Id,
                };

                service = await _context.Services.Where(s => s.Name == "Michelin Tyres - Pilot Sport 4 - 215/40R18").FirstOrDefaultAsync();

                var estimateDetail2 = new EstimateDetail
                {
                    CustomerId = customer.Id,
                    EstimateId = estimateResult.Id,
                    Service = service,
                    Quantity = 2,
                    Price = service.PriceWithDiscount * 2,
                    VehicleId = vehicle.Id,
                };

                _context.EstimateDetails.Add(estimateDetail1);
                _context.EstimateDetails.Add(estimateDetail2);

                await _context.SaveChangesAsync();               

            }

        }

        private async Task AddCustomerAsync()
        {
            if (!_context.Customers.Any())
            {
                // Customer 1
                var customerUser1 = new User
                {
                    FirstName = "Goncalo",
                    LastName = "Patricio",
                    Email = "Goncalo@yopmail.com",
                    UserName = "Goncalo@yopmail.com",
                    PhoneNumber = "963236549",
                    Address = "Aveiro"                     
                };

                _context.Customers.Add(new Customer
                {
                    FirstName = "Goncalo",
                    LastName = "Patricio",                 
                    User = customerUser1,                 
                    Email = customerUser1.Email,
                    Nif = "25648975",
                    Address = customerUser1.Address,
                    PhoneNumber = customerUser1.PhoneNumber                     
                });

                await _userHelper.AddUserAsync(customerUser1, "123456");
                await _userHelper.AddUserToRoleAsync(customerUser1, "Customer");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(customerUser1);
                await _userHelper.ConfirmEmailAsync(customerUser1, token);

                // Customer 2

                var customerUser2 = new User
                {
                    FirstName = "Eliane",
                    LastName = "Santo",
                    Email = "Eliane@yopmail.com",
                    UserName = "Eliane@yopmail.com",
                    PhoneNumber = "935641236",
                    Address = "Bombarral"
                };

                _context.Customers.Add(new Customer
                {
                    FirstName = "Eliane",
                    LastName = "Santo",
                    User = customerUser2,
                    Email = customerUser2.Email,
                    Nif = "12564897",
                    Address = customerUser2.Address,
                    PhoneNumber = customerUser2.PhoneNumber
                });

                await _userHelper.AddUserAsync(customerUser2, "123456");
                await _userHelper.AddUserToRoleAsync(customerUser2, "Customer");
                token = await _userHelper.GenerateEmailConfirmationTokenAsync(customerUser2);
                await _userHelper.ConfirmEmailAsync(customerUser2, token);

                await _context.SaveChangesAsync();
            }

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
                    Name = "Michelin Tyres - Pilot Sport 4 - 215/40R18",
                    Description = "Best Michelin summer road tyres",
                    Price = 180.66m,
                    Discount = 10m
                });

                _context.Services.Add(new Service
                {
                    Name = "Oil Change",
                    Description = "We use the oil you choose and the way you want!",
                    Price = 29.90m,
                    Discount = 0m
                });

                _context.Services.Add(new Service
                {
                    Name = "A/C Recharge",
                    Description = "A/C Gas Research",
                    Price = 49.90m,
                    Discount = 10m
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

                var receptionistSpecialties = new List<Specialty>();

                receptionistSpecialties.Add(new Specialty { Name = "Generalist" });
                _context.EmployeesRoles.Add(new Role
                {
                    Specialties = receptionistSpecialties,
                    Name = "Receptionist",
                    PermissionsName = "Receptionist"
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
                // Employee 1
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
                    Email = employeeUser1.Email,
                    Color = "#8F6593"
                });

                await _userHelper.AddUserAsync(employeeUser1, "123456");                
                await _userHelper.AddUserToRoleAsync(employeeUser1, "Technician");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(employeeUser1);
                await _userHelper.ConfirmEmailAsync(employeeUser1, token);

                // Employee 2
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
                    Email = employeeUser2.Email,
                    Color = "#3066BE"
                });

                await _userHelper.AddUserAsync(employeeUser2, "123456");                
                await _userHelper.AddUserToRoleAsync(employeeUser2, "Technician");
                token = await _userHelper.GenerateEmailConfirmationTokenAsync(employeeUser2);
                await _userHelper.ConfirmEmailAsync(employeeUser2, token);

                // Employee 3
                
                var employeeUser3 = new User
                {
                    FirstName = "Pedro",
                    LastName = "Rato",
                    Email = "Pedrorato@yopmail.com",
                    UserName = "Pedrorato@yopmail.com",
                    PhoneNumber = "923548965",
                    Address = "Rua do Alicate"
                };

                _context.Employees.Add(new Employee
                {
                    FirstName = "Pedro",
                    LastName = "Rato",
                    About = "There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum, you need to be sure there isn't anything embarrassing hidden in the middle of text. ",
                    User = employeeUser3,
                    Role = _context.EmployeesRoles.Where(r => r.Name == "Receptionist").FirstOrDefault(),
                    Specialty = _context.Specialties.Where(s => s.Name == "Generalist").FirstOrDefault(),
                    Email = employeeUser3.Email
                });

                await _userHelper.AddUserAsync(employeeUser3, "123456");
                await _userHelper.AddUserToRoleAsync(employeeUser3, "Receptionist");
                token = await _userHelper.GenerateEmailConfirmationTokenAsync(employeeUser3);
                await _userHelper.ConfirmEmailAsync(employeeUser3, token);

                await _context.SaveChangesAsync();

            }
        }
    }
}

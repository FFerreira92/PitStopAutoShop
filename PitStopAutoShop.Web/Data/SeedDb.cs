using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Helpers;
using System;
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


            await AddUserAsync();
            await AddMechanicsAsync();

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



            await _context.SaveChangesAsync();
        }

        private async Task AddMechanicsAsync()
        {
            if (!_context.Mechanics.Any())
            {

                var mechanicUser1 = new User
                {
                    FirstName = "Joaquim",
                    LastName = "Guedes",
                    Email = "joaquimguedes@yopmail.com",
                    UserName = "joaquimguedes@yopmail.com",
                    PhoneNumber = "965897956",
                    Address ="Rua do barco"
                };

                _context.Mechanics.Add(new Mechanic
                {
                    FirstName = "Joaquim",
                    LastName = "Guedes",
                    Specialty = "Electrician",
                    About = "Born in Lisbon, Joaquim Guedes started his electrician carrer in Bosch Car Service in Lisbon...",
                    User = mechanicUser1

                });

                var mechanicUser2 = new User
                {
                    FirstName = "Inacio",
                    LastName = "Torres",
                    Email = "inaciotorres@yopmail.com",
                    UserName = "inaciotorres@yopmail.com",
                    PhoneNumber = "965896425",
                    Address = "Rua da banana"
                };

                _context.Mechanics.Add(new Mechanic
                {
                    FirstName = "Inacio",
                    LastName = "Torres",
                    Specialty = "Mechanic",
                    About = "Born in Setúbal, Inacio Torres studied mechatronics in ATEC and then joined PitStop Auto, with 8 years of working experience..",
                    User = mechanicUser2
                });

                await _context.SaveChangesAsync();

            }
        }
    }
}

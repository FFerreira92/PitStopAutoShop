using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Data.Repositories
{
    public class EmployeesRolesRepository : GenericRepository<Role>, IEmployeesRolesRepository
    {
        private readonly DataContext _context;

        public EmployeesRolesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddSpecialtyAsync(SpecialtyViewModel model)
        {
            var role = await this.GetRoleWithSpecialtiesAsync(model.RoleId);

            if(role == null)
            {
                return;
            }

            role.Specialties.Add(new Specialty
            {
                Name = model.Name,
            });

            _context.EmployeesRoles.Update(role);

            await _context.SaveChangesAsync();            
        }

        public async Task<int> DeleteSpecialtyAsync(Specialty model)
        {
            var role = await _context.EmployeesRoles.Where(er => er.Specialties.Any(s => s.Id == model.Id)).FirstOrDefaultAsync();

            if(role == null)
            {
                return 0;
            }

            _context.Specialties.Remove(model);

            await _context.SaveChangesAsync();

            return role.Id;
        }

        public IEnumerable<SelectListItem> GetComboRoles()
        {
            var list = _context.EmployeesRoles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Insert the Role]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboSpecialty(int roleId)
        {
            var role = _context.EmployeesRoles.Find(roleId);
            var list = new List<SelectListItem>();

            if(role != null)
            {
                list = _context.Specialties.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }).OrderBy(l => l.Text).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "[Select a Specialty]",
                    Value = "0"
                });
            }

            return list;
        }

        public async Task<Role> GetRoleAsync(Specialty model)
        {
            return await _context.EmployeesRoles.Where(r => r.Specialties.Any(s => s.Id == model.Id)).FirstOrDefaultAsync();
        }

        public async Task<int> GetRoleIdWithSpecialtyAsync(int specialtyId)
        {
            var role = await _context.EmployeesRoles.Where(r => r.Specialties.Any(s=> s.Id == specialtyId)).FirstOrDefaultAsync();

            if(role == null)
            {
                return 0;
            }
            else
            {
                return role.Id;
            }
        }

        public IQueryable GetRolesWithSpecialties()
        {
            return _context.EmployeesRoles.Include(r => r.Specialties).OrderBy(s => s.Name);
        }

        public async Task<Role> GetRoleWithSpecialtiesAsync(int id)
        {
            return await _context.EmployeesRoles.Include(r => r.Specialties).Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Specialty> GetSpecialtyAsync(int id)
        {
            return await _context.Specialties.FindAsync(id);
        }

        public async Task<int> UpdateSpecialtyAsync(Specialty model)
        {
            var role = await _context.EmployeesRoles.Where(r => r.Specialties.Any(s => s.Id == model.Id)).FirstOrDefaultAsync();

            if(role == null)
            {
                return 0;
            }

            _context.Specialties.Update(model);
            await _context.SaveChangesAsync();
            return role.Id;
        }
    }
}

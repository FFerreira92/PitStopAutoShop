using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Models;
using System;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmployeesRolesRepository _employeesRolesRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ConverterHelper(IUserHelper userHelper,
                                IEmployeesRolesRepository employeesRolesRepository,
                                IEmployeeRepository employeeRepository)
        {
            _userHelper = userHelper;
            _employeesRolesRepository = employeesRolesRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee> ToEmployee(EmployeeViewModel model,User user, bool isNew)
        {

            var role = await _employeesRolesRepository.GetRoleWithSpecialtiesAsync(model.RoleId);

            if (role == null)
            {
                return null;
            }

            var specialty = await _employeesRolesRepository.GetSpecialtyAsync(model.SpecialtyId);

            if (specialty == null)
            {
                return null;
            }

            var employee = new Employee
            {
                Id = isNew ? 0 : model.EmployeeId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                About = model.About,
                Role = role,
                Specialty = specialty,
                User = user,
                Email = model.Email
                //Falta acrescentar foto/imagem
            };

            if (isNew)
            {
                try
                {
                    await _employeeRepository.CreateAsync(employee);
                    return employee;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return employee;
            

        }

        public EmployeeViewModel ToEmployeeViewModel(Employee employee,bool isNew)
        {
            var model = new EmployeeViewModel
            {
                EmployeeId = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                About = employee.About,
                Address = employee.User.Address,
                Email = employee.Email,
                PhoneNumber = employee.User.PhoneNumber,
                User = employee.User,
                RoleId = employee.Role.Id,
                SpecialtyId = employee.Specialty.Id,
                Roles = _employeesRolesRepository.GetComboRoles(),
                Specialties = _employeesRolesRepository.GetComboSpecialty(isNew? 0 : employee.Role.Id),
                UserId = employee.User.Id
            };

            return model;
        }

        public Role toRole(RoleViewModel model, bool isNew)
        {
            return new Role
            {
                Id = isNew ? 0 : model.RoleId,
                Name = model.Name,
                PermissionsName = model.SelectedPermission                
            };
        }

        public RoleViewModel toRoleViewModel(Role role)
        {
            return new RoleViewModel
            {
                RoleId = role.Id,
                Name = role.Name,
                SelectedPermission = role.PermissionsName,
                Permissions = _userHelper.GetComboExistingRoles()
            };
        }

       
    }
}

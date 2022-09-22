using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmployeesRolesRepository _employeesRolesRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEstimateRepository _estimateRepository;

        public ConverterHelper(IUserHelper userHelper,
                                IEmployeesRolesRepository employeesRolesRepository,
                                IEmployeeRepository employeeRepository, IEstimateRepository estimateRepository)
        {
            _userHelper = userHelper;
            _employeesRolesRepository = employeesRolesRepository;
            _employeeRepository = employeeRepository;
            _estimateRepository = estimateRepository;
        }
       

        public AppointmentViewModel ToAppointmentViewModel(Appointment appointment, bool isNew)
        {          

            var model = new AppointmentViewModel
            {
                AppointmentEndDate = appointment.AppointmentEndDate,
                AppointmentStartDate = appointment.AppointmentStartDate,
                AppointmentServicesDetails = appointment.AppointmentServicesDetails,
                CreatedBy = appointment.CreatedBy,
                CreatedDate = appointment.CreatedDate,
                Customer = appointment.Customer,
                CustomerId = appointment.Customer.Id,
                EmployeeId = appointment.Mechanic.Id,
                Estimate = appointment.Estimate,
                EstimateId = appointment.Estimate.Id,
                Mechanic = appointment.Mechanic,
                Observations = appointment.Observations,
                UpdatedBy = appointment.UpdatedBy,
                UpdatedDate = appointment.UpdatedDate,
                Vehicle = appointment.Vehicle,
                VehicleId = appointment.Vehicle.Id,
                Id = isNew? 0 : appointment.Id,
                Technicians = _employeeRepository.GetComboTechnicians()
            };

            return model;
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
                Email = model.Email,
                Color = model.Color,
                PhotoId = model.PhotoId == Guid.Empty? new Guid(): model.PhotoId,                
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
                UserId = employee.User.Id,
                Color = employee.Color,
                PhotoId = employee.PhotoId                
            };

            return model;
        }

        public async Task<List<EstimateDetailTemp>> ToEstimateDetailTemps(IEnumerable<EstimateDetail> estimateDetails,string username)
        {
            List<EstimateDetailTemp> list = new List<EstimateDetailTemp>();

            foreach(EstimateDetail temp in estimateDetails)
            {
                var estimateDetailTemp = new EstimateDetailTemp
                {
                    CustomerId = temp.CustomerId,
                    Price = temp.Price,
                    Quantity = temp.Quantity,
                    Service = temp.Service,
                    User = await _userHelper.GetUserByEmailAsync(username),
                    VehicleId = temp.VehicleId,
                };

                list.Add(estimateDetailTemp);
            }

            return list;

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

        public Service ToService(ServiceViewModel model, bool isNew)
        {
            return new Service
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                Description = model.Description,
                Discount = model.Discount,
                Price = model.Price
            };

        }

        public ServiceViewModel ToServiceViewModel(Service service)
        {
            return new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Discount = service.Discount,
                Price = service.Price,
            };
        }
    }
}

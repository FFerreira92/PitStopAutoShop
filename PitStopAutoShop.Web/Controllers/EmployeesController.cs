using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System;
using System.Threading.Tasks;
using Vereyon.Web;

namespace PitStopAutoShop.Web.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IEmployeesRolesRepository _employeesRolesRepository;
        private readonly IFlashMessage _flashMessage;

        public EmployeesController(IEmployeeRepository employeeRepository
                                  ,IUserHelper userHelper
                                  ,IConverterHelper converterHelper
                                  ,IEmployeesRolesRepository employeesRolesRepository
                                  ,IFlashMessage flashMessage)                                  
        {
            _employeeRepository = employeeRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _employeesRolesRepository = employeesRolesRepository;
            _flashMessage = flashMessage;
        }

        public IActionResult Index()
        {
            var employees = _employeeRepository.GetAllWithUsers();

            return View(employees);
        }

        public IActionResult ManageRoles()
        {
            var roles = _employeesRolesRepository.GetRolesWithSpecialties();

            return View(roles);
        }

        public IActionResult CreateRole()
        {
            var model = new RoleViewModel
            {
                Permissions = _userHelper.GetComboExistingRoles()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            model.Permissions = _userHelper.GetComboExistingRoles();

            if (ModelState.IsValid)
            {

                if(model.SelectedPermission == "0")
                {
                    ModelState.AddModelError(string.Empty, "You must select a permission level.");
                    return View(model);
                }

                var roleName = await _userHelper.GetRoleNameByRoleIdAsync(model.SelectedPermission);

                if (string.IsNullOrEmpty(roleName))
                {
                    _flashMessage.Danger("There was an error adding the permission level");
                    return View(model);
                }

                model.SelectedPermission = roleName;

                var role = _converterHelper.toRole(model, true);
                //var role = new Role
                //{
                //    Name = model.Name,
                //    PermissionsName = roleName
                //};

                try
                {
                    await _employeesRolesRepository.CreateAsync(role);
                    return RedirectToAction("ManageRoles","Employees");
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error creating the role. " + ex.InnerException.Message);
                    return View(model);
                }            
            }
            return View(model);
        }

        public async Task<IActionResult> EditRole(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var role = await _employeesRolesRepository.GetRoleWithSpecialtiesAsync(id.Value);

            if(role == null)
            {
                return NotFound();
            }

            var model = _converterHelper.toRoleViewModel(role);

            //convert from roleName to roleId so the dropdown menu in the ViewModel is automatically filled with the current roleName/PermissionLevel
            model.SelectedPermission = await _userHelper.GetRoleIdWithRoleNameAsync(model.SelectedPermission);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleViewModel model)
        {
            model.Permissions = _userHelper.GetComboExistingRoles();

            if (ModelState.IsValid)
            {
                if (model.SelectedPermission == "0")
                {
                    ModelState.AddModelError(string.Empty, "You must select a permission level.");
                    return View(model);
                }

                var roleToEdit = await _employeesRolesRepository.GetByIdAsync(model.RoleId);

                if(roleToEdit == null)
                {
                    return NotFound();
                }

                var roleName = await _userHelper.GetRoleNameByRoleIdAsync(model.SelectedPermission);

                if (string.IsNullOrEmpty(roleName))
                {
                    _flashMessage.Danger("There was an error adding the permission level");
                    return View(model);
                }

                roleToEdit.Name = model.Name;
                roleToEdit.PermissionsName = roleName;

                try
                {
                    await _employeesRolesRepository.UpdateAsync(roleToEdit);
                    return RedirectToAction(nameof(ManageRoles));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error updating the role. " + ex.InnerException.Message);
                    return View(model);
                }

            }

            return View(model);
        }


        public async Task<IActionResult> DeleteRole(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var role = await _employeesRolesRepository.GetRoleWithSpecialtiesAsync(id.Value);

            if (role == null)
            {
                return NotFound();
            }

            try
            {
                await _employeesRolesRepository.DeleteAsync(role);
                _flashMessage.Confirmation("Role was deleted with success.");
            }
            catch (Exception ex)
            {
                _flashMessage.Warning("There was an error deleting the role. " + ex.InnerException.Message);
            }

            return RedirectToAction(nameof(ManageRoles));
        }

        public async Task<IActionResult> RoleDetails(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var role = await _employeesRolesRepository.GetRoleWithSpecialtiesAsync(id.Value);

            if(role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        public async Task<IActionResult> AddSpecialty(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var role = await _employeesRolesRepository.GetRoleWithSpecialtiesAsync(id.Value);

            if(role == null)
            {
                return NotFound();
            }

            var model = new SpecialtyViewModel
            {
                RoleId = role.Id,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddSpecialty(SpecialtyViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _employeesRolesRepository.AddSpecialtyAsync(model);
                return RedirectToAction("RoleDetails", new { id = model.RoleId });
            }

            return View(model);
        }

        public async Task<IActionResult> EditSpecialty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _employeesRolesRepository.GetSpecialtyAsync(id.Value);

            if(specialty == null)
            {
                return NotFound();
            }

            var roleId = await _employeesRolesRepository.GetRoleIdWithSpecialtyAsync(specialty.Id);

            if (roleId == 0)
            {
                return NotFound();
            }

            var specialtyToEdit = new SpecialtyViewModel
            {
                Name = specialty.Name,
                RoleId = roleId,
                SpecialtyId = specialty.Id
            };

            return View(specialtyToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> EditSpecialty(SpecialtyViewModel model)
        {
            if (ModelState.IsValid)
            {

                var modelToChange = await _employeesRolesRepository.GetSpecialtyAsync(model.SpecialtyId);

                if (modelToChange == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error updating the specialty.");
                    return View(model);
                }

                modelToChange.Name = model.Name;

                try
                {
                    await _employeesRolesRepository.UpdateSpecialtyAsync(modelToChange);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    return View(model);
                }

                return RedirectToAction($"RoleDetails", new { id = model.RoleId });
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteSpecialty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _employeesRolesRepository.GetSpecialtyAsync(id.Value);

            if (specialty == null)
            {
                return NotFound();
            }

            var roleId = await _employeesRolesRepository.GetRoleIdWithSpecialtyAsync(specialty.Id);

            if (roleId == 0)
            {
                return NotFound();
            }

            try
            {
                await _employeesRolesRepository.DeleteSpecialtyAsync(specialty);
                return RedirectToAction($"RoleDetails", new { id = roleId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException.Message);
            }

            return RedirectToAction($"RoleDetails", new { id = roleId });
        }

    }
}

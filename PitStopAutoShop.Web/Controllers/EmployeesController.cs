using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System;
using System.Linq;
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
        private readonly IMailHelper _mailHelper;

        public EmployeesController(IEmployeeRepository employeeRepository
                                  ,IUserHelper userHelper
                                  ,IConverterHelper converterHelper
                                  ,IEmployeesRolesRepository employeesRolesRepository
                                  ,IFlashMessage flashMessage
                                  ,IMailHelper mailHelper)                                  
        {
            _employeeRepository = employeeRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _employeesRolesRepository = employeesRolesRepository;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
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

        public IActionResult Create()
        {

            var model = new EmployeeViewModel
            {
                Roles = _employeesRolesRepository.GetComboRoles(),
                Specialties = _employeesRolesRepository.GetComboSpecialty(0)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                };

                var result = await _userHelper.AddUserAsync(newUser, "DefaultPassword123");                

                if(!result.Succeeded)
                {
                    _flashMessage.Danger("There was an error creating the Employee user data.");
                    return View(model);
                }

                var employee = await _converterHelper.ToEmployee(model, newUser, true);

                if(employee == null)
                {
                    _flashMessage.Danger("There was an error creating the employee.");
                    return View(model);
                }
              
                result = await _userHelper.AddUserToRoleAsync(newUser, employee.Role.PermissionsName);

                if (!result.Succeeded)
                {
                    _flashMessage.Danger("There was an error adding the user to the required permission level.");
                    return View(model);
                }

                var userToken = await _userHelper.GenerateEmailConfirmationTokenAsync(newUser);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = newUser.Id,
                    token = userToken
                }, protocol: HttpContext.Request.Scheme);

                Response isSent = _mailHelper.SendEmail(model.Email, "Welcome to PitStop Auto Lisbon", $"<h1>Email Confirmation</h1>" +
                   $"Welcome to PitStop Auto!</br></br>First of all congratulations! You are now a new PitStop AutoStop employee! </br>" +
                   $"To allow you to access the website and the management system, " +
                   $"please click in the following link:<a href= \"{tokenLink}\"> Confirm Email </a>");                            

                if (isSent.IsSuccess)
                {
                    _flashMessage.Confirmation("Employee was created with success! Please allow employee to know that he needs to confirm the email address!");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _flashMessage.Warning("There was an error sending the confirmation email. But the employee has been created. Ask System manager to validate the email address or Delete the employee and try adding him later.");
                    return View(model);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id.Value);

            if(employee == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEmployeeViewModel(employee,false);

            if(model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);

                if(user == null)
                {
                    _flashMessage.Warning("There was an error updating the employee");
                    return View(model);
                }                                

                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                if(user.Email != model.Email)
                {
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    await _userHelper.ConfirmEmailAsync(user, token);
                }

                var result = await _userHelper.UpdateUserAsync(user);

                if (!result.Succeeded)
                {
                    _flashMessage.Warning("There was an error updating the employee user information.");
                    return View(model);
                }


                var employee = await _converterHelper.ToEmployee(model,user,false);

                if(employee == null)
                {
                    _flashMessage.Warning("There was an error updating the employee.");
                    return View(model);
                }

                try
                {
                    await _employeeRepository.UpdateAsync(employee);
                    _flashMessage.Confirmation("Employee was sucessufuly updated.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error updating the employee. "+ex.InnerException.Message);
                    return View(model);
                }              

            }

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEmployeeViewModel(employee, false);

            return View(model);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id.Value);

            if(employee == null)
            {
                return NotFound();
            }

            try
            {
                await _employeeRepository.DeleteAsync(employee);
                _flashMessage.Confirmation("Employee was sucessufuly deleted.");               
            }
            catch (Exception ex)
            {
                _flashMessage.Danger("There was a problem deleting the employee. "+ ex.InnerException.Message);                
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [Route("Employees/GetSpecialtiesAsync")]
        public async Task<JsonResult> GetSpecialtiesAsync(int roleId)
        {
            var role = await _employeesRolesRepository.GetRoleWithSpecialtiesAsync(roleId);
            return Json(role.Specialties.OrderBy(s => s.Name));
        }
    }
}

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
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IFlashMessage _flashMessage;

        public CustomerController(
            ICustomerRepository customerRepository
            , IUserHelper userHelper
            ,IMailHelper mailHelper
            ,IVehicleRepository vehicleRepository
            ,IBrandRepository brandRepository
            ,IFlashMessage flashMessage
        )
        {
            _customerRepository = customerRepository;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _flashMessage = flashMessage;
        }

        public IActionResult Index()
        {
            return View(_customerRepository.GetAll().OrderBy(c => c.FirstName));
        }

        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                bool userExists = false;
                Response isSent = new Response();

                if (user != null)
                {
                   userExists = true;
                }

                var customer = await _customerRepository.GetCustomerByEmailAsync(model.UserName);

                if (customer != null)
                {
                    ModelState.AddModelError(string.Empty, "Customer email already in use.");
                    return View(model);
                }

                if (!userExists)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.UserName,
                        UserName = model.UserName,
                        Address = model.Address
                    };

                    var response = await _userHelper.AddUserAsync(user, "DefaultPassword123");

                    if (!response.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to create User");
                        return View(model);
                    }

                    var result = await _userHelper.AddUserToRoleAsync(user, "Customer");

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created, failed to assign as customer");
                        return View(model);
                    }
                    
                    string userToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = user.Id,
                        token = userToken
                    }, protocol: HttpContext.Request.Scheme);

                    isSent = _mailHelper.SendEmail(model.UserName, "Welcome to PitStop Auto Lisbon", $"<h1>Email Confirmation</h1>" +
                    $"Welcome to PitStop Auto!</br></br>Since you have ordered a service with the best auto shop in Lisbon we created you an account!</br>" +
                    $"To allow you to access the website, " +
                    $"please click in the following link to finish the process:<a href= \"{tokenLink}\"> Confirm Email </a>");                    
                }             
               
                customer = new Customer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Email = model.UserName,
                    User = user,
                    Nif = model.Nif,
                    PhoneNumber = model.PhoneNumber,
                };

                try
                {
                    await _customerRepository.CreateAsync(customer);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    return View(model);
                }                

                var confirmNewCustomerExists = await _customerRepository.CheckIfCustomerInBdByEmailAsync(model.UserName);

                if (!confirmNewCustomerExists)
                {
                    ModelState.AddModelError(string.Empty, "The customer couldn't be created, failed to add to Database");
                    return View(model);
                }                

                if (isSent.IsSuccess)
                {
                    _flashMessage.Confirmation($"Customer account has been created and an email has been sent to {user.Email}, please inform the customer about account creation!");
                    return RedirectToAction("Create", "Vehicles", new { email = user.Email, isEstimate = true});

                }
                else
                {
                    _flashMessage.Warning($"Customer has been created! But failed to send the confirmation email to the client!");
                    return RedirectToAction("Create", "Vehicles",new { email = user.Email, isEstimate = true });
                }
            }

            return View(model);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound(); // alterar para erro personalizado
            }

            var customer = await _customerRepository.GetCustomerWithUserByIdAsync(id.Value);

            if(customer == null)
            {
                return NotFound(); // alterar para erro personalizado
            }

            var model = new EditCustomerViewModel
            {
                Address = customer.Address,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Nif = customer.Nif,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                UserId = customer.User.Id               
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userHelper.GetUserByIdAsync(model.UserId);

                //if(user == null)
                //{
                //    ModelState.AddModelError(string.Empty, "There was an error updating user info. User not found.");
                //    return View(model);
                //}

                //user.FirstName = model.FirstName;
                //user.LastName = model.LastName;
                //user.PhoneNumber = model.PhoneNumber;
                //user.Address = model.Address;
                //user.Email = model.Email;
                //user.UserName = model.Email;
                
                //await _userHelper.UpdateUserAsync(user);
                //user updated
                //------------ / Fará sentido fazer update também aos dados do user? / ----------------------
                //------------/ se decidir alterar dados de user tambem, não esquecer colocar confirmação do novo email do user automáticamente / ------

                var customer = await _customerRepository.GetCustomerByUserIdAsync(model.UserId);

                if(customer == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error updating customer info. Customer not found.");
                    return View(model);
                }

                customer.Address = model.Address;
                customer.Email = model.Email;
                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                customer.PhoneNumber = model.PhoneNumber;
                customer.Nif = model.Nif;

                try
                {
                    await _customerRepository.UpdateAsync(customer);
                }
                catch (Exception ex) 
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    return View(model);
                }                

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        public async Task<IActionResult> Details(int? id)
        {

            if(id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetCustomerWithUserByIdAsync(id.Value);

            if(customer == null)
            {
                return NotFound();
            }

            if(customer.Vehicles.Count > 0)
            {
                foreach(var vehicle in customer.Vehicles)
                {
                    var customerVehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(vehicle.Id);

                    vehicle.Brand = customerVehicle.Brand;
                    vehicle.Model = customerVehicle.Model;
                }
            }


            return View(customer);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetCustomerWithUserByIdAsync(id.Value);


            if(customer == null)
            {
                return NotFound();
            }

            try
            {
                await _customerRepository.DeleteAsync(customer);
                //deveria remover a conta de utilizador tambem? 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);                
            }

            return RedirectToAction(nameof(Index));
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;
using Vereyon.Web;
using Newtonsoft.Json;

namespace PitStopAutoShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IBlobHelper _blobHelper;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public AccountController(IUserHelper userHelper,IMailHelper mailHelper,ICustomerRepository customerRepository,
            IFlashMessage flashMessage, IBlobHelper blobHelper, IInvoiceRepository invoiceRepository, IVehicleRepository vehicleRepository)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _customerRepository = customerRepository;
            _flashMessage = flashMessage;
            _blobHelper = blobHelper;
            _invoiceRepository = invoiceRepository;
            _vehicleRepository = vehicleRepository;
        }

        public IActionResult Login()
        {

            if (User.Identity.IsAuthenticated)
            {
               
                return RedirectToAction("Index", "Home");

            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
           
                var result = await _userHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                    
                    if(user != null)
                    {
                        var isCustomerRole = await _userHelper.CheckUserInRoleAsync(user, "Customer");

                        if (isCustomerRole)
                        {
                            return this.RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Index", "DashboardPanel");
                        }                                            
                    }

                    ModelState.AddModelError(string.Empty, "Failed to Login");                   
                }
            }

            ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);

                if(user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.UserName,
                        UserName = model.UserName,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                    };


                    var result = await _userHelper.AddUserAsync(user, "DefaultPassword123");

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }

                    var customer = new Customer
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.UserName,
                        Address = model.Address,
                        User = user
                    };

                    await _customerRepository.CreateAsync(customer);

                    result = await _userHelper.AddUserToRoleAsync(user, "Customer");

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

                    Response response = await _mailHelper.SendEmail(model.UserName, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $" To allow you to access the website, " +
                        $"please click in the following link:</br></br><a href= \"{tokenLink}\">Confirm Email </a>",null);

                    if (response.IsSuccess)
                    {
                        ViewBag.Message = $"An email has been sent to {user.Email}, please check your email and follow the instructions.";
                        return View(model);
                    }                   

                }

            }

            ModelState.AddModelError(string.Empty, "The user couldn't be created, probably Email is alredy registered.");

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }


            var model = new AddUserPasswordViewModel
            {
                UserId = userId,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(AddUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);
                if(user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user,"DefaultPassword123", model.Password);
                    if (result.Succeeded)
                    {

                        ViewBag.Success = "You can now login into the system.";
                        return View(model);
                        
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found");
                }
            }
            return View(model);
        }


        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "The Email does not correspond to a registered email.");
                    return View(model);
                }

                var userToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", "Account", new { token = userToken,userId = user.Id }, protocol: HttpContext.Request.Scheme);

                Response response = await _mailHelper.SendEmail(model.Email, "PitStop Lisbon Recover Password ", $"<h1>PitStop Lisbon password reset</h1>" +
                    $"To Reset the password click in the link bellow: </br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>",null);

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to recover your password have been sent to the email address.";
                }

                return View();
            }

            return View(model);
        } 

        public async Task<IActionResult> ResetPassword(string token,string userId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                UserName = user.UserName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                if (user != null)
                {
                    var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if (user.EmailConfirmed == false)
                        {
                            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                            await _userHelper.ConfirmEmailAsync(user, token);
                        }

                        ViewBag.Message = "Password Reset Successful, you can now Login with the new credentials.";
                        return View();
                    }


                    ViewBag.Message = "There was an error while resseting your password.";
                    return View(model);
                }

                ViewBag.Message = "User was not found";
                return View(model);
            }

            return View(model);
        }                
             


        [HttpPost]
        [Route("Account/ChangePassword")]
        public async Task<JsonResult> ChangePassword(string oldPassword, string newPassword,string repeatedPassword)
        {

            Response response;

            if(newPassword != repeatedPassword)
            {
                response = new Response
                {
                    IsSuccess = false,
                    Message = "New password is not equivalent to the repeated password."
                };

                return Json(response);
            }         

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if(user == null)
            {
                return Json(new Response
                {
                    IsSuccess = false,
                    Message = "User is not valid",
                });
            }

            var isActualUser = await _userHelper.CheckPasswordAsync(user, oldPassword);

            if (isActualUser.Succeeded)
            {

                var result = await _userHelper.ChangePasswordAsync(user, oldPassword, newPassword);
                if (result.Succeeded)
                {
                    return Json(new Response { IsSuccess = true, Message="Password was succefully changed!" });
                }
                else
                {
                    return Json(new Response
                    {
                        IsSuccess = false,
                        Message = "There was a problem changing the password.",
                    });
                }
            }

            return Json(new Response
            {
                IsSuccess = false,
                Message = "The old password is not correct.",
            });

        }
        

        public async Task<IActionResult> ViewUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if(user == null)
            {
                return NotFound();
            }

            ChangeUserViewModel model = null;
            bool isCustomer =false;

            if (this.User.IsInRole("Customer"))
            {
                var customer = await _customerRepository.GetCustomerByUserIdAsync(user.Id);
                isCustomer = true;
                if(customer == null)
                {
                    return NotFound();
                }

                model = new ChangeUserViewModel
                {
                    PhoneNumber = customer.PhoneNumber,
                    FirstName = customer.FirstName,
                    Address = customer.Address,
                    Email = customer.Email,
                    LastName = customer.LastName,
                    Nif = customer.Nif,
                    ProfilePitcure = user.ProfilePitcure,
                };

               
            }
            else
            {

                model = new ChangeUserViewModel
                {
                    PhoneNumber = user.PhoneNumber,
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    Email = user.Email,
                    UserName = user.UserName,
                    ProfilePitcure = user.ProfilePitcure,
                    Address = user.Address,
                };
                                       
            }

            ViewBag.JsonModel = JsonConvert.SerializeObject(model);
            ViewBag.IsCustomer = JsonConvert.SerializeObject(isCustomer);



            Random r = new Random();
            string[] images = new string[5] { "/images/siteContent/ponte.jpg", "/images/siteContent/recoverPassword.jpg", "/images/siteContent/teste.jpg",
                 "/images/siteContent/karts.jpg", "/images/siteContent/city.jpg" };
            string selectedImage = images[r.Next(5)];            
            ViewBag.Image = selectedImage;

            return View(model);
        }

        public async Task<IActionResult> ServiceHistory()
        {           
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if(user == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetCustomerByUserIdAsync(user.Id);
            
            if(customer == null)
            {
                return NotFound();
            }

            var invoiceHistory = await _invoiceRepository.GetUserInvoicesAsync(customer.Id);

            return View(invoiceHistory);

        }

        public async Task<IActionResult> InvoiceDetails(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var invoice = await _invoiceRepository.GetInvoiceDetailsByIdAsync(id.Value);

            if (invoice == null)
            {
                return NotFound();
            }


            return View(invoice);
        }

        public async Task<IActionResult> Vehicles()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if(user == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetCustomerByUserIdAsync(user.Id);

            if(customer == null)
            {
                return NotFound();
            }

            var vehicles = _vehicleRepository.GetCustomerVehiclesAsync(customer.Id);

            return View(vehicles);

        }


        [HttpPost]
        [Route("Account/GetProfilePicturePath")]
        public async Task<JsonResult> GetProfilePicturePath()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            var json = Json(user);
            return json;
        }

        [HttpPost]
        [Route("Account/ChangeProfilePic")]
        public async Task<IActionResult> ChangeProfilePic(IFormFile file)
        {            
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            
            if(user != null && file != null)
            {
                
                Guid imageId = user.ProfilePitcure;

                if(file != null && file.Length > 0)
                {


                    using var image = Image.Load(file.OpenReadStream());
                    image.Mutate(img => img.Resize(256, 0));
                    
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.SaveAsJpeg(m);
                        byte[] imageBytes = m.ToArray();
                        imageId = await _blobHelper.UploadBlobAsync(imageBytes, "profilepictures");
                    }                 
                }

                user.ProfilePitcure = imageId;

                var response = await _userHelper.UpdateUserAsync(user);
                
                if (!response.Succeeded)
                {
                    _flashMessage.Danger("There was an error updating the profile picture.");
                   
                    return new ObjectResult(new { Status = "fail" });
                }

                return new ObjectResult(new { Status = "success" });
            }


            return new ObjectResult(new { Status = "fail" });
        }



        [HttpPost]
        [Route("Account/UpdateUser")]
        public async Task<JsonResult> UpdateUser(string email,long phoneNumber,long nif,string address,bool isCustomer)
        {
            bool isValid = false;
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return Json(isValid); 
            }

            if (isCustomer)
            {
                var customer = await _customerRepository.GetCustomerByUserIdAsync(user.Id);
                if (customer == null)
                {
                    return Json(isValid);
                }
                user.Email = email;
                user.UserName = email;
                customer.Email = email;
                customer.PhoneNumber = phoneNumber.ToString();
                customer.Address = address;
                customer.Nif = nif.ToString();

                try
                {
                    await _customerRepository.UpdateAsync(customer);
                }
                catch (Exception)
                {
                    return Json(isValid);
                }
            }        
            
            user.Address = address;
            user.PhoneNumber = phoneNumber.ToString();

            try
            {
                await _userHelper.UpdateUserAsync(user);

                isValid = true;
            }
            catch (Exception)
            {
                return Json(isValid);               
            }
           
            return Json(isValid);
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}

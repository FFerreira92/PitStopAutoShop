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

namespace PitStopAutoShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IBlobHelper _blobHelper;

        public AccountController(IUserHelper userHelper,IMailHelper mailHelper,ICustomerRepository customerRepository,
            IFlashMessage flashMessage, IBlobHelper blobHelper)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _customerRepository = customerRepository;
            _flashMessage = flashMessage;
            _blobHelper = blobHelper;
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

                    Response response = _mailHelper.SendEmail(model.UserName, "Email confirmation", $"<h1>Email Confirmation</h1>" +
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
                        return RedirectToAction("AccountCreated", "Account");
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

        public IActionResult AccountCreated()
        {
            return View();
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

                Response response = _mailHelper.SendEmail(model.Email, "PitStop Lisbon Recover Password ", $"<h1>PitStop Lisbon password reset</h1>" +
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
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if(user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {                    
                    ViewBag.Message = "Password Reset Successful, you can now Login with the new credentials.";
                    return View();                    
                }


                ViewBag.Message = "There was an error while resseting your password.";
                return View(model);
            }

            ViewBag.Message = "User was not found";
            return View(model);
        }


        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            var model = new ChangeUserViewModel();
            if(user != null)
            {
                model.Address = user.Address;
                model.PhoneNumber = user.PhoneNumber;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user != null)
                {                    

                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
               

                    var response = await _userHelper.UpdateUserAsync(user);
                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User Updated!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                if(user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }

            }

            return View(model);
        }


        public async Task<IActionResult> ViewUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if(user == null)
            {
                return NotFound();
            }

            return View(user);
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

        public IActionResult NotAuthorized()
        {
            return View();
        }

    }
}

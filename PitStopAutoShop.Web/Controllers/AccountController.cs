using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserHelper userHelper,IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
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

                    return this.RedirectToAction("Index", "Home");
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
                        $"please click in the following link:</br></br><a href= \"{tokenLink}\">Confirm Email </a>");

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

        public IActionResult NotAuthorized()
        {
            return View();
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public CustomerController(ICustomerRepository customerRepository
                                    , IUserHelper userHelper
                                    ,IMailHelper mailHelper)
        {
            _customerRepository = customerRepository;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
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
        public async Task<IActionResult> Create(CreateCustomerViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);

                if(user != null)
                {
                    ModelState.AddModelError(string.Empty, "Customer email already in use.");
                    return View(model);
                }

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

                var customer = new Customer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Email = model.UserName,
                    User = user,
                    Nif = model.Nif,
                    PhoneNumber = model.PhoneNumber,
                };

                await _customerRepository.CreateAsync(customer);

                var confirmNewCustomerExists = await _customerRepository.CheckIfCustomerInBdByEmailAsync(model.UserName);

                if (!confirmNewCustomerExists)
                {
                    ModelState.AddModelError(string.Empty, "The customer couldn't be created, failed to add to Database");
                    return View(model);
                }

                string userToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = userToken
                }, protocol: HttpContext.Request.Scheme);

                Response isSent = _mailHelper.SendEmail(model.UserName, "Welcome to PitStop Auto Lisbon", $"<h1>Email Confirmation</h1>" +
                    $"Welcome to PitStop Auto!</br></br>Since you have ordered a service with the best auto shop in Lisbon we created you an account!</br>" +
                    $"To allow you to access the website, " +
                    $"please click in the following link to finish the process:<a href= \"{tokenLink}\"> Confirm Email </a>");

                if (isSent.IsSuccess)
                {
                    ViewBag.Message = $"Customer account has been created and an email has been sent to {user.Email}, please inform the customer about account creation!";
                    return View(model);
                }
            }

            return View(model);
        }

    }
}

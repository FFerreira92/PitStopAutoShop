﻿using Microsoft.AspNetCore.Mvc;
using PitStopAutoShop.Web.Helpers;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserHelper _userHelper;

        public UserController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetails(string email)
        {
            var results = await _userHelper.GetUserDetailsAsync(email);
            var result = Ok(results);
            return result;
        }

    }
}

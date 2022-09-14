using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user,string password);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<User> GetUserByIdAsync(string userId);

        Task CheckRoleAsync(string roleName);

        Task<bool> CheckUserInRoleAsync(User user, string roleName);

        Task<IdentityResult> AddUserToRoleAsync(User user, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        IEnumerable<SelectListItem> GetComboExistingRoles();

        Task<string> GetRoleNameByRoleIdAsync(string roleId);

        Task<string> GetRoleIdWithRoleNameAsync(string roleName);
        Task<List<UserDataChartModel>> GetUsersChartDataAsync();
        Task<int> GetTotalUsersAsync();
    }
}

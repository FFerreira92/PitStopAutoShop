using Microsoft.AspNetCore.Identity;
using PitStopAutoShop.Web.Data.Entities;
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
    }
}

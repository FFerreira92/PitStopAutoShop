using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;

namespace PitStopAutoShop.Web.Helpers
{
    public interface IConverterHelper
    {
        Role toRole(RoleViewModel model, bool isNew);

        RoleViewModel toRoleViewModel(Role role);
 
    }
}

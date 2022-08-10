using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Models;

namespace PitStopAutoShop.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly IUserHelper _userHelper;

        public ConverterHelper(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }
               

        public Role toRole(RoleViewModel model, bool isNew)
        {
            return new Role
            {
                Id = isNew ? 0 : model.RoleId,
                Name = model.Name,
                PermissionsName = model.SelectedPermission                
            };
        }

        public RoleViewModel toRoleViewModel(Role role)
        {
            return new RoleViewModel
            {
                RoleId = role.Id,
                Name = role.Name,
                SelectedPermission = role.PermissionsName,
                Permissions = _userHelper.GetComboExistingRoles()
            };
        }

       
    }
}

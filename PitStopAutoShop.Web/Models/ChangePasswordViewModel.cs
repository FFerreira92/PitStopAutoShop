using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        [Display(Name = "Repeat New Password")]
        public string Confirm { get; set; }
    }
}

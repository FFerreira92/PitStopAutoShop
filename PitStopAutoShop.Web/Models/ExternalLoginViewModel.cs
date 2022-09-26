using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Models
{
    public class ExternalLoginViewModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}

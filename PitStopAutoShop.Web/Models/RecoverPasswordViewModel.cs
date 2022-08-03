using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Models
{
    public class RecoverPasswordViewModel
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

    }
}

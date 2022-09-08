using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PitStopAutoShop.Web.Models
{
    public class ChangeUserViewModel
    {        

        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Address { get; set; }

        [Display(Name ="Phone Number")]
        [MaxLength(20, ErrorMessage = "The field {0} can only contain {1} characters.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} must be numeric")]        
        public string PhoneNumber { get; set; }

    }
}

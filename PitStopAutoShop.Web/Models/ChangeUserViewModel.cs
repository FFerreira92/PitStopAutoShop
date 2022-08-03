using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PitStopAutoShop.Web.Models
{
    public class ChangeUserViewModel
    {        

        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Address { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string PhoneNumber { get; set; }


    }
}

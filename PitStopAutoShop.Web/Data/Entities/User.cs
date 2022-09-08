using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [MaxLength(100,ErrorMessage ="The field {0} cannot have more then {1} characters.")]
        public string Address { get; set; }


        [Display(Name ="Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public override string PhoneNumber { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        
        public string ProfilePitcure { get; set; }

        public string ProfilePictureAltPath => ProfilePitcure == null ? null : ProfilePitcure.Substring(1);

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ProfilePitcure))
                {
                    return null;
                }

                return $"https://pitstopautoshop.azurewebsites.net/{ProfilePitcure.Substring(1)}";
            }
        }
    }
}

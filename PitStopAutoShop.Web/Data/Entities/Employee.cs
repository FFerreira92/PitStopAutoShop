using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class Employee : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "First Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Last Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        public Specialty Specialty { get; set; }
        
        [Required(ErrorMessage = "Must insert the {0}.")]
        public Role Role { get; set; }

        public string About { get; set; }

        [Display(Name = "Photo")]
        public string? ImageUrl { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public User User { get; set; }

        public string Color { get; set; }
    }
}

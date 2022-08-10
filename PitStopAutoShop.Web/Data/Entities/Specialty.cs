using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class Specialty : IEntity
{
        public int Id { get; set; }

        [Required]
        [Display(Name = "Specialty")]
        [MaxLength(50, ErrorMessage = "The {0} may only contain {1} characters.")]
        public string Name { get; set; }

    }
}

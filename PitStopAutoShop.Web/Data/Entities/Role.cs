using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class Role : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The {0} may only contain {1} characters.")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Permissions")]
        public string PermissionsName { get; set; }

        public ICollection<Specialty> Specialties { get; set; }

        public int NumberOfSpecialties => Specialties == null ? 0 : Specialties.Count;
    }
}

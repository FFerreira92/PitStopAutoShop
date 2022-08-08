using System;
using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class Vehicle : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Must insert the {0}.")]
        [MaxLength(50)]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [MaxLength(50)]
        public string Model { get; set; }

        [Display(Name = "Date of Construction")]
        [DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}", ApplyFormatInEditMode =false)]
        public DateTime DateOfConstruction { get; set; }

        [Required]
        [MaxLength(8,ErrorMessage ="{0} can only have {1} characters.")]
        [MinLength(6,ErrorMessage = "{0} needs to have at least {1} characters.")]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }

        [MaxLength(17)]
        [Display(Name ="VIN")]
        public string VehicleIdentificationNumber { get; set; }
                
        public int Horsepower { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public string BrandAndModel => $"{Brand} - {Model}";
    }
}

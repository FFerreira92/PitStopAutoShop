using PitStopAutoShop.Web.Data.CustomValidation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        public string Observations { get; set; }

        public Employee Mechanic { get; set; }

        public Customer Customer { get; set; }

        public Vehicle Vehicle { get; set; }

        public User CreatedBy { get; set; }

        public User UpdatedBy { get; set; }

        [Display(Name = "Scheduled Date")]
        [Required(ErrorMessage = "Must insert the {0}")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]        
        [NotOnSundaysValidator(ErrorMessage = "The Shop is closed on Sundays")]
        public DateTime AppointmentDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

    }
}

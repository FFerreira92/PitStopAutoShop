using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PitStopAutoShop.Web.Models
{
    public class AddCustomerAndVehicleToEstimateViewModel
    {
        //public IEnumerable<SelectListItem> Customers { get; set; }

        //public List<Customer> Customers { get; set; }      

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Customer")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Customer")]
        public int CustomerId { get; set; }

        public IEnumerable<SelectListItem> Vehicles { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Vehicle")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Vehicle")]
        public int VehicleId { get; set; }
    }
}

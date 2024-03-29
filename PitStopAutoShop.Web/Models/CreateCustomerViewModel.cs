﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PitStopAutoShop.Web.Models
{
    public class CreateCustomerViewModel : RegisterViewModel
    {
        [Display(Name = "Tax Identification Number / NIF")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} must be numeric")]        
        public string Nif { get; set; }



    }
}

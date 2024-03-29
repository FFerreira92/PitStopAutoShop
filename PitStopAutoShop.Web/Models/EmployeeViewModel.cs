﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PitStopAutoShop.Web.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "First Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Last Name")]
        [MaxLength(25, ErrorMessage = "the field {0} can only contain {1} characters.")]
        public string LastName { get; set; }

        public string About { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Address { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "{0} must be numeric")]
        [Display(Name = "Phone Number")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Role")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Role")]
        public int RoleId { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        [Display(Name = "Specialty")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Specialty")]
        public int SpecialtyId { get; set; }

        public IEnumerable<SelectListItem> Specialties { get; set; }

        public string Color { get; set; }

        public Guid PhotoId { get; set; }
       
    }
}

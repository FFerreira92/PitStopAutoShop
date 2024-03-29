﻿using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PitStopAutoShop.Web.Models
{
    public class AppointmentViewModel : Appointment
    {

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Technician")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Technician.")]
        public int EmployeeId { get; set; }

        public int EstimateId { get; set; }
       
        public int CustomerId { get; set; }

        public int VehicleId { get; set; }        

        public IEnumerable<SelectListItem> Technicians { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;
using System;

namespace PitStopAutoShop.Web.Models
{
    public class APIUserViewModel
    {
        public string FirstName { get; set; }
           
        public string LastName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public Guid ProfilePitcure { get; set; }
    }
}

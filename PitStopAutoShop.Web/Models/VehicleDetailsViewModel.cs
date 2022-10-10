﻿using System;
using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Models
{
    public class VehicleDetailsViewModel
    {

        public string BrandName { get; set; }

        public string ModelName { get; set; }

        public string DateOfConstruction { get; set; }

        public string PlateNumber { get; set; }

        public string VIN { get; set; }

        public string HorsePower { get; set; }


    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class EstimateDetailTemp :IEntity 
    {
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Service Service { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

        public decimal Value => Price * (decimal)Quantity;
    }
}

﻿using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class Estimate : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Estimate Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime EstimateDate { get; set; }

        [Required]
        public Customer Customer { get; set; }    

        [Required]
        public User CreatedBy { get; set; }

        [Required]
        public Vehicle Vehicle { get; set; }
       
        public IEnumerable<EstimateDetail> Services { get; set; }

        [Display(Name ="Fault Description")]
        public string FaultDescription { get; set; }

        [Display(Name ="Nº Services")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity => Services == null ? 0 : Services.Sum(s => s.Quantity);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value => Services.Sum(s => s.Value);
        
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public double ValueWithDiscount => Services.Sum(s=> s.ValueWithDiscount);

        public bool HasAppointment { get; set; }
    }
}

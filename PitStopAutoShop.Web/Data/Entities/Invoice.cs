using System;
using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }

        public Customer Customer { get; set; }

        public Vehicle Vehicle { get; set; }

        public Estimate Estimate { get; set; }

        public WorkOrder WorkOrder { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value { get; set; }       

        public User CreatedBy { get; set; }

        [Display(Name ="Date")]
        public DateTime InvoicDate { get; set; }
    }
}

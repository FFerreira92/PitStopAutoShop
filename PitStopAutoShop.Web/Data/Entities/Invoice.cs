using System;

namespace PitStopAutoShop.Web.Data.Entities
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }

        public Customer Customer { get; set; }

        public Vehicle Vehicle { get; set; }

        public Estimate Estimate { get; set; }

        public WorkOrder WorkOrder { get; set; }

        public string Observations { get; set; }

        public decimal Value { get; set; }       

        public User CreatedBy { get; set; }

        public DateTime InvoicDate { get; set; }
    }
}

using System;

namespace API.Models
{
    public class CapitalPriceTrackingDto
    {
        public Guid WarehousingId { get; set; }

        public Guid BillId { get; set; }

        public Guid InventoryId { get; set; }

        public double Amount { get; set; }

        public double InputPrice { get; set; }

        public double CapitalPrice { get; set; }

        public double Inventory { get; set; }
      
    }
}

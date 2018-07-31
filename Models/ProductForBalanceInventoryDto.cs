using System;

namespace API.Models
{
    public class ProductForBalanceInventoryDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int SystemInventory { get; set; }

        public int RealInventory { get; set; }

        public string Reason { get; set; }
    }
}

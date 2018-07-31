using API.Infrastructure;
using System;

namespace API.Models
{
    public class InventoryDto: CodeBaseDto
    {
      
        public UserDto CreatedUser { get; set; }

        public Guid CreatedUserId { get; set; }

        [Sortable]
        [Filterable]
        public DateTime CreatedDateTime { get; set; }

        public UserDto BalanceUser { get; set; }

        public Guid BalancedUserId { get; set; }

        [Sortable]
        [Filterable]
        public DateTime BalanceDateTime { get; set; }

        [Sortable]
        [Filterable]
        public string Note { get; set; }

        [Sortable]
        [Filterable]
        public double TotalDeviation { get; set; }

        [Sortable]
        [Filterable]
        public double IncreaseDeviation { get; set; }

        [Sortable]
        [Filterable]
        public int DecreaseDeviation { get; set; }

        [Sortable]
        [Filterable]
        public string Status { get; set; } // INVENTORY, BALANCE, DESTROY

        public string ProductList { get; set; }

     
        public StorageDto Storage { get; set; }

        public Guid StorageId { get; set; }
    }
}

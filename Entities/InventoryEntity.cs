using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class InventoryEntity : CodeBaseEntity
    {
        [Required]
        public UserEntity CreatedUser { get; set;}

        [Required]
        public Guid CreatedUserId { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; }

        public UserEntity BalanceUser { get; set; }

        public Guid? BalanceUserId { get; set; }

        public DateTime BalanceDateTime { get; set; }

        public string Note { get; set; }

        [Required]
        public double TotalDeviation { get; set; }

        [Required]
        public double IncreaseDeviation { get; set; }

        [Required]
        public double DecreaseDeviation { get; set; }

        [Required]
        public string Status { get; set; } // INVENTORY, BALANCE, DESTROY

        [Required]
        public string ProductList { get; set; }

        [Required]
        public StorageEntity Storage { get; set; }

        [Required]
        public Guid StorageId { get; set; }

    }
}

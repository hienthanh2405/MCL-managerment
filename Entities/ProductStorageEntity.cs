using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class ProductStorageEntity : BaseEntity
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Guid StorageId { get; set; }

        public string ProductProductionDateList { get; set; }

        [Required]
        public ProductEntity Product { get; set; }
        [Required]
        public StorageEntity Storage { get; set; }
        [Required]
        public double Inventory { get; set; }

        public double CapitalPrice { get; set; }

        public string PlacePosition { get; set; }

        public string CapitalPriceTrackings { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class ProductEntity : CodeBaseEntity
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public double SalePrice { get; set; }

        [Required]
        public double WholeSalePrice { get; set; }

        [Required]
        public DateTime UpdatePriceDate { get; set; }

        [Required]
        public Guid ProviderId { get; set; }

        [Required]
        public Guid ProductCategoryId { get; set; }

        public string Note { get; set; }

        [Required]
        public ProductCategoryEntity ProductCategory { get; set; }

        [Required]
        public ProviderEntity Provider { get; set; }

        [Required]
        public ICollection<ProductStorageEntity> ProductStorages { get; set; }

        [Required]
        public string Unit { get; set; }

        public string UnitConverterList { get; set; }

    }
}

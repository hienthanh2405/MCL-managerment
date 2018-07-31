using API.Infrastructure;
using System;
using System.Collections.Generic;

namespace API.Models
{
    public class ProductStorageDto : BaseDto
    {
        public Guid ProductId { get; set; }

        public List<ProductProductionDateDto> ProductProductionDateList { get; set; }

        public Guid StorageId { get; set; }
      
        public ProductDto Product { get; set; }
      
        public StorageDto Storage { get; set; }

        [Sortable]
        [FilterableDecimal]
        public double Inventory { get; set; }

        [Sortable]
        [Filterable]
        public double CapitalPrice { get; set; }

        public string PlacePosition { get; set; }
    }
}

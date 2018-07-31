using API.Entities;
using API.Infrastructure;
using System;
using System.Collections.Generic;

namespace API.Models
{
    public class ProductDto : BaseDto
    {
        [Sortable]
        [Filterable]
        public string Code { get; set; }

        [Sortable]
        [Filterable]
        public string Name { get; set; }

        [Sortable]
        [Filterable]
        public double CapitalPrice { get; set; }

        [Sortable]
        [Filterable]
        public double SalePrice { get; set; }


        [Sortable]
        [Filterable]
        public double WholeSalePrice { get; set; }

        [Sortable]
        [Filterable]
        public DateTime UpdatePriceDate { get; set; }

        [Sortable]
        [Filterable]
        public string Note { get; set; }

        public Guid ProviderId { get; set; }

        public Guid ProductCategoryId { get; set; }

        public double Inventory { get; set; }

        public string PlacePosition { get; set; }

        public StorageDto Storage { get; set; }

        public ProductCategoryDto ProductCategory { get; set; }

        public ProviderDto Provider { get; set; }

        public string StorageName { get; set; }

        public List<ProductProductionDateDto> ProductProductionDateList { get; set; }

        [Sortable]
        [Filterable]
        public string Unit { get; set; }

        public List<UnitConverterDto> UnitConverterList { get; set; }

        public bool HasDiscount { get; set; }

        public double Discount { get; set; }

        public bool IsMoneyDiscount { get; set; }
    
        public double SalePriceAfterDiscount { get; set; }
    }
}

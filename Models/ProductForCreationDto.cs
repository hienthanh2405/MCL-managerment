using API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class ProductForCreationDto : CodeBaseDto
    {

        [Required(ErrorMessage = "You must provide a Name value.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must provide a Capital Price value.")]
        public double CapitalPrice { get; set; }

        [Required(ErrorMessage = "You must provide a SalePrice value.")]
        public double SalePrice { get; set; }

        [Required(ErrorMessage = "You must provide a WholeSalePrice value.")]
        public double WholeSalePrice { get; set; }

        [Required(ErrorMessage = "You must provide a ProviderId Date value.")]
        public Guid ProviderId { get; set; }

        [Required(ErrorMessage = "You must provide a ProductCategoryId value.")]
        public Guid ProductCategoryId { get; set; }

        public string Note { get; set; }

        [Required(ErrorMessage = "You must provide a Invetory value.")]
        public double Inventory { get; set; }

        public string PlacePosition { get; set; }

        [Required(ErrorMessage = "You must provide a CurrentStorageId value.")]
        public Guid CurrentStorageId { get; set; }

        public List<ProductProductionDateDto> ProductProductionDateList { get; set; }
        
        [Required(ErrorMessage = "You must provide a CurrentStorageId BasicUnit.")]
        public string Unit { get; set; }

        public List<UnitConverterDto> UnitConverterList { get; set; }
    }
}

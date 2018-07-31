using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ProductStorageForCreationDto
    {
        [Required(ErrorMessage = "You should provide a ProductId value.")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "You should provide a StorageId value.")]
        public Guid StorageId { get; set; }

        public string ProductProductionDateList { get; set; }

        [Required]
        public double Inventory { get; set; }

        public string PlacePosition { get; set; }

    }
}

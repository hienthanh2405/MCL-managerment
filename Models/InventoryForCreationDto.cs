using API.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class InventoryForCreationDto: CodeBaseDto
    {
        [Required(ErrorMessage = "You must provide CreatedUserId")]
        public Guid CreatedUserId { get; set; }

        public string Note { get; set; }

        [Required(ErrorMessage = "You must provide TotalDeviation")]
        public double TotalDeviation { get; set; }

        [Required(ErrorMessage = "You must provide DescreaseDeviation")]
        public double IncreaseDeviation { get; set; }

        [Required(ErrorMessage = "You must provide DescreaseDeviation")]
        public double DecreaseDeviation { get; set; }

        [Required(ErrorMessage = "You must provide ProductList")]
        public string ProductList { get; set; }

        [Required(ErrorMessage = "You must provide StorageId")]
        public Guid StorageId { get; set; }

    }
}

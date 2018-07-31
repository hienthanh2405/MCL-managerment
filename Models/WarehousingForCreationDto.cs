using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class WarehousingForCreationDto
    {
        [Required(ErrorMessage = "You should provide a InputDate value.")]
        public DateTime InputDate { get; set; }

        [Required(ErrorMessage = "You should provide a ProductList value.")]
        public List<ProductForWareshousingCreationDto> ProductList { get; set; }

        [Required(ErrorMessage = "You should provide a PaymentMoney value.")]
        public double PaymentMoney { get; set; }

        [Required(ErrorMessage = "You should provide a TaxMoney value.")]
        public double TaxMoney { get; set; }

        public DateTime UpdatedDateTime { get; set; }

        public List<SupplierBillDto> SupplierBillList { get; set; }

        public double DebtDays { get; set; }

        public string Note { get; set; }

        [Required(ErrorMessage = "You should provide a StorageId value.")]
        public Guid StorageId { get; set; }

        [Required(ErrorMessage = "You should provide a SupplierId value.")]
        public Guid SupplierId { get; set; }

    }
}

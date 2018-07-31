using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class WarehousingEntity : BaseEntity
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public DateTime InputDate { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }

        public string SupplierBillList { get; set; }

        [Required]
        public double ProductMoney { get; set; }

        [Required]
        public double TaxMoney { get; set; }

        [Required]
        public double SummaryMoney { get; set; }

        [Required]
        public double PaymentMoney { get; set; }

        [Required]
        public double DebtMoney { get; set; }

        public double DebtDays { get; set; }

        public string Note { get; set; }

        [Required]
        public Guid StorageId { get; set; }

        [Required]
        public Guid SupplierId { get; set; }

        [Required]
        public String ProductList { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public Guid CreatedUserId { get; set;}

        public UserEntity CreatedUser { get; set; }

        public Guid? UpdatedUserId { get; set; }

        public UserEntity UpdatedUser { get; set; }

        public StorageEntity Storage { get; set; }

        public SupplierEntity Supplier { get; set; }

    }
}

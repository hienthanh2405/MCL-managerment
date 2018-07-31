using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class BillEntity : CodeBaseEntity
    {
        [Required]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        public CustomerEntity Customer { get; set; }

        [Required]
        public Guid StorageId { get; set; }

        public StorageEntity Storage { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public UserEntity User { get; set; }

        [Required]
        public string ProductList { get; set; }

        [Required]
        // Total Money with current sale price of products
        public double NoEditTotalMoney { get; set; }

        [Required]
        public double ShipMoney { get; set; }

        [Required]
        // money from accumulation points
        public double PointMoney { get; set; }

        [Required]
        // Total Money with discount (using accumulation points). This is the final total money.
        // TotalMoney =  ( NoEditTotalMoney - PointMoney)
        public double TotalMoney { get; set; }

        [Required]
        // the money of guest passed to staff. We use it to calculate the ReturnMoney
        public double GuestPaymentMoney { get; set; }

        [Required]
        // The real input money at transaction
        // If(GuestDebtMoney>0) PaymentMoney = GuestPaymentMoney
        // Else PaymentMoney = TotalMoney
        public double PaymentMoney { get; set; }

        [Required]
        public double PaymentCash { get; set; }

        [Required]
        public double PaymentCard { get; set; }

        [Required]
        public double PaymentBank { get; set; }

        [Required]
        // GuestDebtMoney = TotalMoney - PaymentMoney
        public double GuestDebtMoney { get; set; }

        [Required]
        //ReturnMoney = (GuestPaymentMoney - TotalMoney)
        public double ReturnMoney { get; set; }

        [Required]
        public double UsedPoints { get; set; }

        [Required]
        public double PointEarning { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsRetail { get; set; }

        public string Note { get; set; }

        public string ShipAddress { get; set; }

        public string ShipPhone { get; set; }

        public string ShipContactName { get; set; }

        public string AttachedDocuments { get; set; }

        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyTaxCode { get; set; }

        public bool HaveAssembly { get; set; }

        public bool HaveTireChange { get; set; }

        public string Seller { get; set; }
    }
}

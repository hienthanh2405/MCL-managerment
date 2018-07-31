using API.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class BillForCreationDto : BaseDto
    {

        [Required(ErrorMessage = "You must provide CreatedDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [Required(ErrorMessage = "You must provide CustomerId")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "You must provide ProductList")]
        public List<ProductForBillCreationDto> ProductList { get; set; }

        [Required(ErrorMessage = "You must provide NoEditTotalMoney")]
        public double NoEditTotalMoney { get; set; }

        [Required(ErrorMessage = "You must provide ShipMoney")]
        public double ShipMoney { get; set; }

        [Required(ErrorMessage = "You must provide TotalMoney")]
        public double TotalMoney { get; set; }

        [Required(ErrorMessage = "You must provide PaymentMoney")]
        public double PaymentMoney { get; set; }

        [Required(ErrorMessage = "You must provide TotalMoney")]
        public double PaymentCash { get; set; }

        [Required(ErrorMessage = "You must provide PaymentCard")]
        public int PaymentCard { get; set; }

        [Required(ErrorMessage = "You must provide PaymentBank")]
        public double PaymentBank { get; set; }

        [Required(ErrorMessage = "You must provide ReturnMoney")]
        public double ReturnMoney { get; set; }

        [Required(ErrorMessage = "You must provide UsedPoints")]
        public int UsedPoints { get; set; }

        [Required(ErrorMessage = "You must provide StorageId")]
        public Guid StorageId { get; set; }

        public string Note { get; set; }

        public string ShipAddress { get; set; }

        public string ShipPhone { get; set; }

        public string ShipContactName { get; set; }

        public string AttachedDocuments { get; set; }

        [Required(ErrorMessage = "You must provide IsRetail")]
        public bool IsRetail { get; set; }

        [Required(ErrorMessage = "You must provide GuestDebtMoney")]
        public double GuestDebtMoney { get; set; }

        [Required(ErrorMessage = "You must provide GuestPaymentMoney")]
        public double GuestPaymentMoney { get; set; }

        public bool IsUpdatedCustomerShipping { get; set; }

        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyTaxCode { get; set; }

        public bool HaveAssembly { get; set; }

        public bool HaveTireChange { get; set; }

        public string Seller { get; set; }
    }
}

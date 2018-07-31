using API.Infrastructure;
using System;
using System.Collections.Generic;

namespace API.Models
{
    public class BillDto : CodeBaseDto
    {
        [Sortable]
        [Filterable]
        public DateTime CreatedDateTime { get; set; }

        public Guid CustomerId { get; set; }

        public CustomerDto Customer { get; set; }

        public List<ProductForBillCreationDto> ProductList { get; set; }

        [Sortable]
        [Filterable]
        public double NoEditTotalMoney { get; set; }

        [Sortable]
        [Filterable]
        public double ShipMoney { get; set; }

        [Sortable]
        [Filterable]
        public double PointMoney { get; set; }

        [Sortable]
        [Filterable]
        public double TotalMoney { get; set; } 

        [Sortable]
        [Filterable]
        public double GuestPaymentMoney { get; set; } 

        [Sortable]
        [Filterable]
        public double PaymentMoney { get; set; }

        [Sortable]
        [Filterable]
        public double PaymentCash { get; set; }

        [Sortable]
        [Filterable]
        public double PaymentCard { get; set; }

        [Sortable]
        [Filterable]
        public double PaymentBank { get; set; }

        [Sortable]
        [Filterable]
        public double ReturnMoney { get; set; } 

        [Sortable]
        [Filterable]
        public double GuestDebtMoney { get; set; }

        [Sortable]
        [Filterable]
        public double UsedPoints { get; set; }

        [Sortable]
        [Filterable]
        public double PointEarning { get; set; }

        public Guid StorageId { get; set; }

        public StorageDto Storage { get; set; }

        public Guid UserId { get; set; }

        public UserDto User { get; set; }

        [Sortable]
        [Filterable]
        public bool IsActive { get; set; }

        [Sortable]
        [Filterable]
        public string Note { get; set; }

        [Sortable]
        [Filterable]
        public string ShipAddress { get; set; }

        [Sortable]
        [Filterable]
        public string ShipPhone { get; set; }

        [Sortable]
        [Filterable]
        public string ShipContactName { get; set; }

        [Sortable]
        [Filterable]
        public string AttachedDocuments { get; set; }

        [Sortable]
        [Filterable]
        public bool IsRetail { get; set; }


        [Sortable]
        [Filterable]
        public string CompanyName { get; set; }

        [Sortable]
        [Filterable]
        public string CompanyAddress { get; set; }

        [Sortable]
        [Filterable]
        public string CompanyTaxCode { get; set; }

        [Sortable]
        [Filterable]
        public bool HaveAssembly { get; set; }

        [Sortable]
        [Filterable]
        public bool HaveTireChange { get; set; }

        [Sortable]
        [Filterable]
        public string Seller { get; set; }

    }

    public class BillForGetSingleDto : BillDto
    {
        public new List<ProductForBillGetSingleDto> ProductList { get; set; }
    }
}

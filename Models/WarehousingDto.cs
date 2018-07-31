using API.Infrastructure;
using System;
using System.Collections.Generic;

namespace API.Models
{
    public class WarehousingDto : BaseDto
    {
        [Sortable]
        [Filterable]
        public string Code { get; set; }

        [Sortable]
        [Filterable]
        public DateTime InputDate { get; set; }

        [Sortable]
        [Filterable]
        public DateTime CreatedDateTime { get; set; }

        [Sortable]
        [Filterable]
        public DateTime UpdatedDateTime { get; set; }

        public List<ProductForWareshousingCreationDto> ProductList { get; set; }

        public List<SupplierBillDto> SupllierBillList { get; set; }

        [Sortable]
        [Filterable]
        public double ProductMoney { get; set; }

        [Sortable]
        [Filterable]
        public double TaxMoney { get; set; }

        [Sortable]
        [Filterable]
        public double SummaryMoney { get; set; }

        [Sortable]
        [Filterable]
        public double PaymentMoney { get; set; }

        [Sortable]
        [Filterable]
        public double DebtMoney { get; set; }

        [Sortable]
        [Filterable]
        public double DebtDays { get; set; }

        [Sortable]
        [Filterable]
        public string Note { get; set; }

        [Sortable]
        [Filterable]
        public bool IsActive { get; set; }

        public Guid StorageId { get; set; }

        public Guid SupplierId { get; set; }

        public Guid CreatedUserId { get; set; }

        public Guid UpdatedUserId { get; set; }

        public UserDto CreatedUser { get; set; }

        public UserDto UpdatedUser { get; set; }

        public StorageDto Storage { get; set; }

        public SupplierDto Supplier { get; set; }

    }
}

using System;
using System.Collections.Generic;

namespace API.Models
{
    public class ProductForBillCreationDto : BaseDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public double CapitalPrice { get; set; }

        public double Amount { get; set; }

        public string Unit { get; set; }

        public double SalePrice { get; set; }

        public double WholeSalePrice { get; set; }

        public double ListedWholeSalePrice { get; set; }

        public double TotalMoney { get; set; }

        public List<DetailAmountDto> DetailAmountList { get; set; }

        public string SerialNumber { get; set; }

        public bool IsService { get; set; }

    }

    public class ProductForBillGetSingleDto : ProductForBillCreationDto
    {
        public new List<ReturnDetailAmountDto> DetailAmountList { get; set; }
    }
}

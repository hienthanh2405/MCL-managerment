using System;
using System.Collections.Generic;

namespace API.Models
{
    public class WarehousingReportDto
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public double WarehousingAmount { get; set; }

        public double ProductAmount { get; set; }

        public double ProductMoney { get; set; }

        public double TaxMoney { get; set; }

        public double SummaryMoney { get; set; }

        public double PaymentMoney { get; set; }

        public double DebtMoney { get; set; }

        public List<ReturnStorageWarehousingDto> StorageWarehousingList { get; set; }
    }

    public class ReturnStorageWarehousingDto
    {
        public string StorageName { get; set; }

        public List<WarehousingDto> WarehousingList { get; set; }

    }
}

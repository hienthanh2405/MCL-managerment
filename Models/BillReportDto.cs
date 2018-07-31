using System;
using System.Collections.Generic;

namespace API.Models
{
    public class BillReportDto
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int BillAmount { get; set; }

        public int RetailBillAmount { get; set; }

        public int WholeSaleBillAmount { get; set; }

        public double ProductAmount { get; set; }

        public double NoEditTotalMoney { get; set; }

        public double ShipMoney { get; set; }

        public double PointMoney { get; set; }

        public double TotalMoney { get; set; }

        public double PaymentMoney { get; set; }

        public double PaymentCash { get; set; }
        
        public double PaymentCard { get; set; }

        public double PaymentBank { get; set; }

        public double GuestDebtMoney { get; set; }

        public List<ReturnStorageBillDto> StorageBillList { get; set; }
    }

    public class ReturnStorageBillDto
    {
        public string StorageName { get; set; }

        public List<BillDto> BillList { get; set; }

    }
}

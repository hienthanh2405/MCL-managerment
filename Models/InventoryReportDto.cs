using System;
using System.Collections.Generic;

namespace API.Models
{
    public class InventoryReportDto
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public List<string> StorageNameList { get; set; }

        public double InventoryAmount { get; set; }

        public double InventoriedInventoryAmount { get; set; }

        public double BalanceInventoryAmount { get; set; }

        public double TotalDeviation { get; set; }

        public double DecreaseDeviation { get; set; }

        public double IncreaseDeviation { get; set; }

        public List<ReturnStorageInventoryDto> ReturnStorageInventoryList { get; set; }
    }

    public class ReturnStorageInventoryDto
    {
        public string StorageName { get; set; }

        public List<InventoryDto> InventoryList { get; set; }

    }
}

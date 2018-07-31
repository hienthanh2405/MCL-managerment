using System;
using System.Collections.Generic;

namespace API.Models
{
    public class ProductStorageReportDto
    {
        public DateTime CreatedDateTime { get; set; }

        public double Inventory { get; set; }

        public List<string> StorageNames { get; set; }

        public List<ReturnProductStorageDto> ReturnProductStorageList { get; set; }
    }

    public class ReturnProductStorageDto
    {
        public string StorageName { get; set; }

        public List<ProductStorageDto> ProductStorageList { get; set; }

    }
}

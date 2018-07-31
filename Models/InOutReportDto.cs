using System;

namespace API.Models
{
    public class InOutReportDto
    {
        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public double FirstTermInventory { get; set; }

        public double LastTermInventory { get; set; }

        public double InputAmount { get; set; }

        public double OutputAmount { get; set; }
        
    }
}

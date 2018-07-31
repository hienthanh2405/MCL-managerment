using System;

namespace API.Models
{
    public class ProductProductionDateDto
    {
        public string ProductionWeekYear { get; set; }

        public DateTime ProductionDate { get; set; }

        public double Inventory { get; set; }
    }
}

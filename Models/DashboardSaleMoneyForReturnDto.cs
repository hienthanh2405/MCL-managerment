using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class DashboardSaleMoneyForReturnDto
    {
        public double TotalMoney { get; set; }
        public double TotalCash { get; set; }
        public double TotalBank { get; set; }
        public double TotalCard { get; set; }
    }
}

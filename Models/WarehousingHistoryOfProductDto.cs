using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class WarehousingHistoryOfProductDto
    {
        public DateTime CreatedDate { get; set; }

        public string Code { get; set; }

        public double Amount { get; set; }

        public SupplierDto Supplier { get; set; }
    }
}

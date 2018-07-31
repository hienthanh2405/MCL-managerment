using System;

namespace API.Models
{
    public class BillHistoryOfProductDto
    {

        public DateTime CreatedDate { get; set; }

        public string Code { get; set; }

        public double Amount { get; set; }

        public CustomerDto Customer { get; set; }

    }
}

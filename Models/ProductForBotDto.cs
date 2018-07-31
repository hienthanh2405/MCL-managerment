using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class ProductForBotDto : BaseDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public double CapitalPrice { get; set; }
    }
}

using System.Collections.Generic;

namespace API.Models
{
    public class ProductForWareshousingCreationDto : BaseDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public double InputAmount { get; set; }

        public List<DetailInputAmountDto> DetailInputAmountList { get; set; }
   
        public double InputPrice { get; set; }

    }
}

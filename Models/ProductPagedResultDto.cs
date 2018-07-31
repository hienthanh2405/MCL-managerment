using API.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class ProductPagedResultDto : PagedResults<ProductDto>
    {
        public int OldProductNumber { get; set; }

    }
}

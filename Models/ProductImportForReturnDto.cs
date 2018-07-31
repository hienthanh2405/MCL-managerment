using System.Collections.Generic;

namespace API.Models
{
    public class ProductImportForReturnDto
    {
        public int TotalNumber { get; set; }

        public int SuccessNumber { get; set; }

        public int ErrorNumber { get; set; }

        public int NotFoundNumber { get; set; }

        public List<string> NotFoundItemCodes { get; set; }

        public List<string> ErrorItemCodes { get; set; }
 
    }
}

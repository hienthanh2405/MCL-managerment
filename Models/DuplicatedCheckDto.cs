using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class DuplicatedCheckReturnDto
    {
        public bool IsDuplicated { get; set; }

        public DuplicatedCheckReturnDto(bool isDuplicated)
        {
            IsDuplicated = isDuplicated;
        }
    }
}

using API.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class StorageDto : BaseDto
    {
      
        [SortableAttribute]
        [FilterableAttribute]
        [Required(ErrorMessage = "You should provide a Name value.")]
        public string Name { get; set; }

        [SortableAttribute]
        [FilterableAttribute]
        public string Address { get; set; }
    }
}

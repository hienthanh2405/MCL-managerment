using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class ProductCategoryForCreationDto
    {
        [Required(ErrorMessage = "You should provide a Name value.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You should provide a Keyword value.")]
        public string Keyword { get; set; }

        public Guid ParentId { get; set; }
    }
}

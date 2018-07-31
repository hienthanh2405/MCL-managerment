using API.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class PageDto : BaseDto
    {
        [Sortable]
        [Filterable]
        [Required(ErrorMessage ="You must provide Index")]
        public int Index { get; set; }

        [Sortable]
        [Filterable]
        [Required(ErrorMessage = "You must provide Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must provide ValidRoleNames")]
        public string ValidRoleNames { get; set; }

        

    }
}

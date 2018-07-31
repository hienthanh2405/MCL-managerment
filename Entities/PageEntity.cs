using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class PageEntity : BaseEntity
    {
        [Required]
        public int Index { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ValidRoleNames { get; set; }
        // list of role names which can access a page

    }
}

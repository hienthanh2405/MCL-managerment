using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class ProductCategoryEntity : BaseEntity
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Keyword { get; set; }

        public Guid? ParentId { get; set; }

    }
}

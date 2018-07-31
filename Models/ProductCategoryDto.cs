using API.Infrastructure;
using System;

namespace API.Models
{
    public class ProductCategoryDto
    {
        public Guid Id { get; set; }

        [Sortable]
        [Filterable]
        public string Name { get; set; }

        public string Keyword { get; set; }

        public Guid ParentId { get; set; }


    }
}

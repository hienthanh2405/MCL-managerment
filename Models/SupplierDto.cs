using API.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class SupplierDto
    {
        public Guid Id { get; set; }

        [SortableAttribute]
        [FilterableAttribute]
        [Required(ErrorMessage = "You should provide a Code value.")]
        public string Name { get; set; }

        [SortableAttribute]
        [FilterableAttribute]
        [Required(ErrorMessage = "You should provide a Name value.")]
        public string Code { get; set; }

        [SortableAttribute]
        [FilterableAttribute]
        [Required(ErrorMessage = "You should provide a Phone value.")]
        public string Phone { get; set; }

        [SortableAttribute]
        [FilterableAttribute]
        [Required(ErrorMessage = "You should provide a Address value.")]
        public string Address { get; set; }

        [SortableAttribute]
        [Required(ErrorMessage = "You should provide a TaxCode value.")]
        public string TaxCode { get; set; }

        [SortableAttribute]
        [FilterableAttribute]
        public string Email { get; set; }

        [SortableAttribute]
        [FilterableAttribute]
        public string Note { get; set; }



    }
}

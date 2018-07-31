using API.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class CustomerDto: CodeBaseDto
    {
        [Required(ErrorMessage = "You must provide Code")]
        [Sortable]
        [Filterable]
        public new string Code { get; set; } // This is a bull shit for sorting by Code. I don't why can not use Sortable on CodeBaseDto

        [Required(ErrorMessage = "You must provide FirstName")]
        [Sortable]
        [Filterable]
        public string FirstName { get; set; }

        [Sortable]
        [Filterable]
        public string LastName { get; set; }

        [Sortable]
        [Filterable]
        public string Phone { get; set; }

        [Required(ErrorMessage = "You must provide IsMale")]
        [Sortable]
        [Filterable]
        public bool IsMale { get; set; }

        [Sortable]
        [Filterable]
        public string Email { get; set; }

        [Sortable]
        [Filterable]
        public DateTime BirthDate { get; set; }

        [Sortable]
        [Filterable]
        public string Address { get; set; }


        [Sortable]
        [Filterable]
        public string CompanyName { get; set; }

        [Sortable]
        [Filterable]
        public string CompanyAddress { get; set; }

        [Sortable]
        [Filterable]
        public string CompanyTaxCode { get; set; }

        [Sortable]
        [Filterable]
        public string CompanyPhone { get; set; }

        [Sortable]
        [Filterable]
        public string CompanyEmail { get; set; }

        [Sortable]
        [Filterable]
        public string ShipAddress { get; set; }

        [Sortable]
        [Filterable]
        public string ShipPhone { get; set; }

        [Sortable]
        [Filterable]
        public string ShipContactName { get; set; }

        [Sortable]
        [Filterable]
        public string Note { get; set; }

        [Sortable]
        [Filterable]
        public double AccumulationPoint { get; set; }
    }
}

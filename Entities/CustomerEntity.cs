using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class CustomerEntity : CodeBaseEntity
    {
        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        [Required]
        public bool IsMale { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; }

        public string Address { get; set; }

        public string CompanyName { get; set;}

        public string CompanyAddress { get; set; }

        public string CompanyTaxCode { get; set; }

        public string CompanyPhone { get; set; }

        public string CompanyEmail { get; set; }

        public string ShipAddress { get; set; }

        public string ShipPhone { get; set; }

        public string ShipContactName { get; set; }

        public string Note { get; set; }

        [Required]
        public double AccumulationPoint { get; set; }

    }
}

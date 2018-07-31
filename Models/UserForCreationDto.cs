using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserForCreationDto
    {

        [Required(ErrorMessage = "You must provide a Username value.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "You must provide a FirstName value.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You must provide a LastName value.")]
        public string LastName { get; set; }

        public string JobTitle { get; set; }

        [Required(ErrorMessage = "You should provide a Password value.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "You should provide a Email value.")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "You should provide a RoleNames value.")]
        public IEnumerable<string> RoleNames { get; set; }


        [Required(ErrorMessage = "You should provide a StorageIds value.")]
        public IEnumerable<Guid> StorageIds { get; set; }
    }
}

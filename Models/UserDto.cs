using API.Infrastructure;
using System.Collections.Generic;

namespace API.Models
{
    public class UserDto : BaseDto
    {
        [Sortable]
        [Filterable]
        public string UserName { get; set; }

        [Sortable]
        [Filterable]
        public string FirstName { get; set; }

        [Sortable]
        [Filterable]
        public string LastName { get; set; }

        [Sortable]
        [Filterable]
        public string JobTitle { get; set; }

        [Sortable]
        [Filterable]
        public string Email { get; set; }

        [Sortable]
        [Filterable]
        public string PhoneNumber { get; set; }

        public IEnumerable<string> RoleNames { get; set; }

        [Sortable]
        [Filterable]
        public bool IsActive { get; set; }

        public IEnumerable<StorageDto> AccessibleStorages { get; set; }



    }
}

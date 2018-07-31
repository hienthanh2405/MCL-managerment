using API.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserStorageDto : BaseDto
    {
        [Required( ErrorMessage = "You must provide UserId")]      
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "You must provide StorageId")]
        public Guid StorageId { get; set; }
      
        public UserDto User { get; set; }
      
        public StorageDto Storage { get; set; }

    }
}

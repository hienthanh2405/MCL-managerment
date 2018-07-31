using System;

namespace API.Entities
{
    public class UserStorageEntity : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid StorageId { get; set; }

        public UserEntity User { get; set; }

        public StorageEntity Storage { get; set; }
    }
}

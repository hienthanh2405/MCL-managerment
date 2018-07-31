using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class StorageEntity: BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public string Address { get; set; }

        [Required]
        public virtual ICollection<ProductStorageEntity> ProductStorages { get; set; }

    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace API.Entities
{
    public class RoleEntity : IdentityRole<Guid>
    {
        public int Index { get; set; }

        public RoleEntity(): base()
        {
        }

        public RoleEntity(string roleName , int index)
            : base(roleName)
        {
            Index = index;
        }
    }
}

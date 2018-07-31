using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class DatabaseContext : IdentityDbContext<UserEntity, RoleEntity, Guid>
    {
        public DatabaseContext(DbContextOptions options)
            : base(options) { }

        public DbSet<ProviderEntity> Providers { get; set; }
        public DbSet<UnitEntity> Units { get; set; }
        public DbSet<ProductCategoryEntity> ProductCategories { get; set; }
        public DbSet<SupplierEntity> Suppliers { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<StorageEntity> Storages { get; set; }
        public DbSet<ProductStorageEntity> ProductStorages { get; set; }
        public DbSet<WarehousingEntity> Warehousings { get; set; }    
        public DbSet<BillEntity> Bills { get; set; }
        public DbSet<SettingEntity> Settings { get; set; }
        public DbSet<PageEntity> Pages { get; set; }
        public DbSet<UserStorageEntity> UserStorages { get; set; }
        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<InventoryEntity> Inventories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

     
}
}

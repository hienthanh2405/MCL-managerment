using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using API.Entities;

namespace API.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("API.Entities.BillEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AttachedDocuments");

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("CompanyAddress");

                    b.Property<string>("CompanyName");

                    b.Property<string>("CompanyTaxCode");

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("CustomerId");

                    b.Property<double>("GuestDebtMoney");

                    b.Property<double>("GuestPaymentMoney");

                    b.Property<bool>("HaveAssembly");

                    b.Property<bool>("HaveTireChange");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsRetail");

                    b.Property<double>("NoEditTotalMoney");

                    b.Property<string>("Note");

                    b.Property<double>("PaymentBank");

                    b.Property<double>("PaymentCard");

                    b.Property<double>("PaymentCash");

                    b.Property<double>("PaymentMoney");

                    b.Property<double>("PointEarning");

                    b.Property<double>("PointMoney");

                    b.Property<string>("ProductList")
                        .IsRequired();

                    b.Property<double>("ReturnMoney");

                    b.Property<string>("Seller");

                    b.Property<string>("ShipAddress");

                    b.Property<string>("ShipContactName");

                    b.Property<double>("ShipMoney");

                    b.Property<string>("ShipPhone");

                    b.Property<Guid>("StorageId");

                    b.Property<double>("TotalMoney");

                    b.Property<double>("UsedPoints");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("StorageId");

                    b.HasIndex("UserId");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("API.Entities.CustomerEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("AccumulationPoint");

                    b.Property<string>("Address");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("CompanyAddress");

                    b.Property<string>("CompanyEmail");

                    b.Property<string>("CompanyName");

                    b.Property<string>("CompanyPhone");

                    b.Property<string>("CompanyTaxCode");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<bool>("IsMale");

                    b.Property<string>("LastName");

                    b.Property<string>("Note");

                    b.Property<string>("Phone");

                    b.Property<string>("ShipAddress");

                    b.Property<string>("ShipContactName");

                    b.Property<string>("ShipPhone");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("API.Entities.InventoryEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("BalanceDateTime");

                    b.Property<Guid?>("BalanceUserId");

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("CreatedUserId");

                    b.Property<double>("DecreaseDeviation");

                    b.Property<double>("IncreaseDeviation");

                    b.Property<string>("Note");

                    b.Property<string>("ProductList")
                        .IsRequired();

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<Guid>("StorageId");

                    b.Property<double>("TotalDeviation");

                    b.HasKey("Id");

                    b.HasIndex("BalanceUserId");

                    b.HasIndex("CreatedUserId");

                    b.HasIndex("StorageId");

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("API.Entities.PageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Index");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ValidRoleNames")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("API.Entities.ProductCategoryEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Keyword")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("ParentId");

                    b.HasKey("Id");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("API.Entities.ProductEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Note");

                    b.Property<Guid>("ProductCategoryId");

                    b.Property<Guid>("ProviderId");

                    b.Property<double>("SalePrice");

                    b.Property<string>("Unit")
                        .IsRequired();

                    b.Property<string>("UnitConverterList");

                    b.Property<DateTime>("UpdatePriceDate");

                    b.Property<double>("WholeSalePrice");

                    b.HasKey("Id");

                    b.HasIndex("ProductCategoryId");

                    b.HasIndex("ProviderId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("API.Entities.ProductStorageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("CapitalPrice");

                    b.Property<string>("CapitalPriceTrackings");

                    b.Property<double>("Inventory");

                    b.Property<string>("PlacePosition");

                    b.Property<Guid>("ProductId");

                    b.Property<string>("ProductProductionDateList");

                    b.Property<Guid>("StorageId");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("StorageId");

                    b.ToTable("ProductStorages");
                });

            modelBuilder.Entity("API.Entities.ProviderEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("API.Entities.RoleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<int>("Index");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("API.Entities.SettingEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CanSaleWithDebtForRetailBill");

                    b.Property<string>("CompanyAddress");

                    b.Property<string>("CompanyDescription");

                    b.Property<string>("CompanyEmail");

                    b.Property<string>("CompanyFax");

                    b.Property<string>("CompanyName");

                    b.Property<string>("CompanyPhone");

                    b.Property<string>("CompanyWebsite");

                    b.Property<bool>("IsAllowNegativeInventoryBill");

                    b.Property<bool>("IsAllowUsingPoint");

                    b.Property<bool>("IsOnlyDestroyBillOnSameDay");

                    b.Property<int>("MoneyToPoint");

                    b.Property<int>("OldProductWarmingDays");

                    b.Property<int>("OldTravelTireWarmingDays");

                    b.Property<int>("OldTruckTireWarmingDays");

                    b.Property<int>("PointToMoney");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("API.Entities.StorageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Storages");
                });

            modelBuilder.Entity("API.Entities.SupplierEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Note");

                    b.Property<string>("Phone")
                        .IsRequired();

                    b.Property<string>("TaxCode")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("API.Entities.UnitEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("API.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsActive");

                    b.Property<string>("JobTitle");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("API.Entities.UserStorageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("StorageId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("StorageId");

                    b.HasIndex("UserId");

                    b.ToTable("UserStorages");
                });

            modelBuilder.Entity("API.Entities.WarehousingEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("CreatedUserId");

                    b.Property<double>("DebtDays");

                    b.Property<double>("DebtMoney");

                    b.Property<DateTime>("InputDate");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Note");

                    b.Property<double>("PaymentMoney");

                    b.Property<string>("ProductList")
                        .IsRequired();

                    b.Property<double>("ProductMoney");

                    b.Property<Guid>("StorageId");

                    b.Property<double>("SummaryMoney");

                    b.Property<string>("SupplierBillList");

                    b.Property<Guid>("SupplierId");

                    b.Property<double>("TaxMoney");

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<Guid?>("UpdatedUserId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedUserId");

                    b.HasIndex("StorageId");

                    b.HasIndex("SupplierId");

                    b.HasIndex("UpdatedUserId");

                    b.ToTable("Warehousings");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<Guid>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictApplication", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId")
                        .IsRequired();

                    b.Property<string>("ClientSecret");

                    b.Property<string>("DisplayName");

                    b.Property<string>("PostLogoutRedirectUris");

                    b.Property<string>("RedirectUris");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.ToTable("OpenIddictApplications");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictAuthorization", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationId");

                    b.Property<string>("Scopes");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("Subject")
                        .IsRequired();

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.ToTable("OpenIddictAuthorizations");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictScope", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.ToTable("OpenIddictScopes");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictToken", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationId");

                    b.Property<string>("AuthorizationId");

                    b.Property<string>("Ciphertext");

                    b.Property<DateTimeOffset?>("CreationDate");

                    b.Property<DateTimeOffset?>("ExpirationDate");

                    b.Property<string>("Hash");

                    b.Property<string>("Status");

                    b.Property<string>("Subject")
                        .IsRequired();

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.HasIndex("AuthorizationId");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.ToTable("OpenIddictTokens");
                });

            modelBuilder.Entity("API.Entities.BillEntity", b =>
                {
                    b.HasOne("API.Entities.CustomerEntity", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId");

                    b.HasOne("API.Entities.StorageEntity", "Storage")
                        .WithMany()
                        .HasForeignKey("StorageId");

                    b.HasOne("API.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("API.Entities.InventoryEntity", b =>
                {
                    b.HasOne("API.Entities.UserEntity", "BalanceUser")
                        .WithMany()
                        .HasForeignKey("BalanceUserId");

                    b.HasOne("API.Entities.UserEntity", "CreatedUser")
                        .WithMany()
                        .HasForeignKey("CreatedUserId");

                    b.HasOne("API.Entities.StorageEntity", "Storage")
                        .WithMany()
                        .HasForeignKey("StorageId");
                });

            modelBuilder.Entity("API.Entities.ProductEntity", b =>
                {
                    b.HasOne("API.Entities.ProductCategoryEntity", "ProductCategory")
                        .WithMany()
                        .HasForeignKey("ProductCategoryId");

                    b.HasOne("API.Entities.ProviderEntity", "Provider")
                        .WithMany()
                        .HasForeignKey("ProviderId");
                });

            modelBuilder.Entity("API.Entities.ProductStorageEntity", b =>
                {
                    b.HasOne("API.Entities.ProductEntity", "Product")
                        .WithMany("ProductStorages")
                        .HasForeignKey("ProductId");

                    b.HasOne("API.Entities.StorageEntity", "Storage")
                        .WithMany("ProductStorages")
                        .HasForeignKey("StorageId");
                });

            modelBuilder.Entity("API.Entities.UserStorageEntity", b =>
                {
                    b.HasOne("API.Entities.StorageEntity", "Storage")
                        .WithMany()
                        .HasForeignKey("StorageId");

                    b.HasOne("API.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("API.Entities.WarehousingEntity", b =>
                {
                    b.HasOne("API.Entities.UserEntity", "CreatedUser")
                        .WithMany()
                        .HasForeignKey("CreatedUserId");

                    b.HasOne("API.Entities.StorageEntity", "Storage")
                        .WithMany()
                        .HasForeignKey("StorageId");

                    b.HasOne("API.Entities.SupplierEntity", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierId");

                    b.HasOne("API.Entities.UserEntity", "UpdatedUser")
                        .WithMany()
                        .HasForeignKey("UpdatedUserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("API.Entities.RoleEntity")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("API.Entities.UserEntity")
                        .WithMany("Claims")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("API.Entities.UserEntity")
                        .WithMany("Logins")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("API.Entities.RoleEntity")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("API.Entities.UserEntity")
                        .WithMany("Roles")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictAuthorization", b =>
                {
                    b.HasOne("OpenIddict.Models.OpenIddictApplication", "Application")
                        .WithMany("Authorizations")
                        .HasForeignKey("ApplicationId");
                });

            modelBuilder.Entity("OpenIddict.Models.OpenIddictToken", b =>
                {
                    b.HasOne("OpenIddict.Models.OpenIddictApplication", "Application")
                        .WithMany("Tokens")
                        .HasForeignKey("ApplicationId");

                    b.HasOne("OpenIddict.Models.OpenIddictAuthorization", "Authorization")
                        .WithMany("Tokens")
                        .HasForeignKey("AuthorizationId");
                });
        }
    }
}

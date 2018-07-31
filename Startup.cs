using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using API.Helpers;

namespace API
{
    public class Startup
    {
        private readonly int? _httpsPort;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Get the HTTPS port (only in development)
            if (env.IsDevelopment())
            {
                var launchJsonConfig = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("Properties\\launchSettings.json")
                    .Build();
                _httpsPort = launchJsonConfig.GetValue<int>("iisSettings:iisExpress:sslPort");
            }
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<DatabaseContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("SecurityConnection"));
                opt.UseOpenIddict();
            });
            // Map some of the default claim names to the proper OpenID Connect claim names
            services.Configure<IdentityOptions>(opt =>
            {
                opt.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                opt.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                opt.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            // Add OpenIddict services
            services.AddOpenIddict<Guid>(opt =>
            {
                opt.AddEntityFrameworkCoreStores<DatabaseContext>();
                opt.AddMvcBinders();

                opt.EnableTokenEndpoint("/token");
                opt.AllowPasswordFlow();
            });


            // Add ASP.NET Core Identity
            services.AddIdentity<UserEntity, RoleEntity>()
                .AddEntityFrameworkStores<DatabaseContext, Guid>()
                .AddDefaultTokenProviders();

            // enable CORS 
            services.AddCors();

            // Add MVC
            services.AddMvc(opt =>
           {
               // Require HTTPS for all controllers
               opt.SslPort = _httpsPort;
               opt.Filters.Add(typeof(RequireHttpsAttribute));
           });

            // add repositoires 

            //services.AddTransient<IProductCategoryRepository, ProductCategoryRepository>();

            services.AddTransient<IDashboardRepository, DashboardRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IInOutReportRepository, InOutReportRepository>();

            services.AddScoped(typeof(IGenericRepository<,,>), typeof(GenericRepository<,,>));
            services.AddScoped(typeof(ICodeGenericRepository<,,>), typeof(CodeGenericRepository<,,>));
            services.AddScoped(typeof(IProductStorageRepository), typeof(ProductStorageRepository));
            services.AddScoped(typeof(IWarehousingRepository), typeof(WarehousingRepository));
            services.AddScoped(typeof(IBillRepository), typeof(BillRepository));
            services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
            services.AddScoped(typeof(IPageRepository), typeof(PageRepository));
            services.AddScoped(typeof(IUserStorageRepository), typeof(UserStorageRepository));
            services.AddScoped(typeof(IInventoryRepository), typeof(InventoryRepository));
            services.AddScoped(typeof(IStorageRepository), typeof(StorageRepository));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add AutoMappers
            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
         
            // Add default data for Page table
            var databaseContext = app.ApplicationServices.GetRequiredService<DatabaseContext>();
            var roleManager = app.ApplicationServices.GetRequiredService<RoleManager<RoleEntity>>();
            var userManager = app.ApplicationServices.GetRequiredService<UserManager<UserEntity>>();

            bool createDefaultData = true; // we can not create default data to start dev server faster
            if(createDefaultData)
            {
                CreateDefaultRoles(roleManager).Wait();
                CreateDefaultPages(databaseContext).Wait();
                CreateDefaultAccessiblePagesForRoles(databaseContext).Wait();
                CreateDefaultProductCategory(databaseContext).Wait();
                CreateDefaultProviders(databaseContext).Wait();
                CreateDefaultUnits(databaseContext).Wait();
                CreateDefaultStorage(databaseContext).Wait();
                CreateDefaultSetting(databaseContext).Wait();

                var user = new UserEntity
                {
                    UserName = "congtymichelin",
                    Email = "congtymichelin@gmail.com",
                    FirstName = "MICHELIN",
                    LastName = "Công ty",
                    JobTitle = "Quản trị viên",
                    IsActive = true
                };
                string password = "Michelin@123";

                AddDefaultUsers(roleManager, userManager, user, password, databaseContext).Wait();

                SetOwnerHasAllStorages(user.UserName, databaseContext).Wait();
            }

            if (env.IsDevelopment())
            {
                var user = new UserEntity
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    FirstName = "Admin",
                    LastName = "Công ty",
                    JobTitle = "Quản trị viên",
                    IsActive = true
                };
                string password = "Admin@123";

                AddDefaultUsers(roleManager, userManager, user, password, databaseContext).Wait();

                SetOwnerHasAllStorages(user.UserName, databaseContext).Wait();
            }

            // Use UseCors
            app.UseCors(
               options => options.WithOrigins("*")
               .AllowAnyMethod()
               .AllowAnyHeader()
            );

            app.UseOAuthValidation();
            app.UseOpenIddict();
            app.UseMvc();
        }
        private static async Task CreateDefaultRoles(RoleManager<RoleEntity> roleManager)
        {
            await roleManager.CreateAsync(new RoleEntity( CONSTANT.ROLE_OWNER , 1));
            await roleManager.CreateAsync(new RoleEntity(CONSTANT.ROLE_MANAGER , 2));
            await roleManager.CreateAsync(new RoleEntity(CONSTANT.ROLE_SALEMAN ,3 ));
            await roleManager.CreateAsync(new RoleEntity(CONSTANT.ROLE_WAREHOUSE ,4 ));
        }

        private static async Task CreateDefaultPages(DatabaseContext context)
        {
            var entity = context.Set<PageEntity>();
            string[] pageNames = new string[]  {"Dashboard", "BillCreate", "BillEdit" , "Bill", "Customer",
                "Product", "Provider", "ProductCategory" , "ProductGroup","ProductStorage", "Warehousing" ,
                "WarehousingCreate", "WarehousingEdit",
                "Inventory" , "InventoryCreate","Supplier", "Storage" ,  "Setting" , "Page" , "User",
                "Promotion" , "InOutReport" , "Service", "Gift", "ReceiveGift", "Feedback"
            };


            // set OWNER can access all pages
            for (int i = 0; i < pageNames.Length; i++)
            {
                var pageName = pageNames[i];
                var currentPage = await entity.SingleOrDefaultAsync(p => p.Name == pageName);
                if (currentPage == null)
                {
                    var newPage = new PageEntity
                    {
                        Name = pageName,
                        Index = i,
                        ValidRoleNames = "[\"OWNER\"]"
                    };
                    entity.Add(newPage);
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task CreateDefaultAccessiblePagesForRoles(DatabaseContext context)
        {
            List<string> salemanAndWarehousePages = new List<string> { "Dashboard", "Product", "Provider", "ProductCategory",
                "ProductGroup", "ProductStorage" , "Service"};

            List<string> salemanPages = new List<string> { "Bill" , "BillCreate" , "BillEdit" , "Customer"};
            salemanPages = salemanPages.Concat(salemanAndWarehousePages).ToList();

            List<string> warehousePages = new List<string> { "Warehousing" , "WarehousingCreate", "WarehousingEdit",
                "Inventory" , "InventoryCreate" , "Supplier" , "InOutReport" };
            warehousePages = warehousePages.Concat(salemanAndWarehousePages).ToList();

            List<string> managerPages = new List<string> { "Storage" , "Promotion" , "Gift" , "ReceiveGift" };
            managerPages = managerPages.Concat(salemanPages).Concat(warehousePages).Distinct().ToList();

            var allPages = await context.Pages.ToListAsync(); 
         
            foreach(var page in allPages)
            {
                var validRoleNames = JsonConvert.DeserializeObject<List<string>>(page.ValidRoleNames);

                if ( managerPages.Contains(page.Name) && !validRoleNames.Contains(CONSTANT.ROLE_MANAGER))
                {
                    validRoleNames.Add(CONSTANT.ROLE_MANAGER);
                }
                if (salemanPages.Contains(page.Name) && !validRoleNames.Contains(CONSTANT.ROLE_SALEMAN))
                {
                    validRoleNames.Add(CONSTANT.ROLE_SALEMAN);
                }
                if (warehousePages.Contains(page.Name) && !validRoleNames.Contains(CONSTANT.ROLE_WAREHOUSE))
                {
                    validRoleNames.Add(CONSTANT.ROLE_WAREHOUSE);
                }
      
                page.ValidRoleNames = JsonConvert.SerializeObject(validRoleNames);
                context.Pages.Update(page);
            }
        
            await context.SaveChangesAsync();
        }

        private static async Task CreateDefaultStorage(DatabaseContext context)
        {
            var entity = context.Set<StorageEntity>();
            string[] storageNameList = new string[] { "123", "BXMD", "BXMT" };

            for (int i = 0; i < storageNameList.Length; i++)
            {
                var name = storageNameList[i];
                var currentPage = await entity.SingleOrDefaultAsync(p => p.Name == name);
                if (currentPage == null)
                {
                    var storage = new StorageEntity
                    {
                        Name = name
                    };
                    await entity.AddAsync(storage);
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task CreateDefaultProductCategory(DatabaseContext context)
        {
            var entity = context.Set<ProductCategoryEntity>();

            string[] productCategoryNameList = new string[] { "PC", "LT", "MAM", "TB" , "DAUNHOT" , "RUOT"  , "YEM"  , "MC" , "MAMCU"};

            for (int i = 0; i < productCategoryNameList.Length; i++)
            {
                var name = productCategoryNameList[i];
                var currentPage = await entity.SingleOrDefaultAsync(p => p.Name == name);
                if (currentPage == null)
                {
                    var productGroup = new ProductCategoryEntity
                    {
                        Name = name,
                        Keyword = name
                    };
                    await entity.AddAsync(productGroup);
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task CreateDefaultProviders(DatabaseContext context)
        {

            List<string> providerNames = new List<string>()
                {
                    "Chưa phân loại", "Michelin", "Bridgestone" , "Fuchs"
                };
            foreach (string name in providerNames)
            {
                var currentProvider = await context.Providers.SingleOrDefaultAsync(p => p.Name == name);
                if (currentProvider == null)
                {
                    var provider = new ProviderEntity()
                    {
                        Name = name
                    };
                    await context.Providers.AddAsync(provider);
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task CreateDefaultSetting(DatabaseContext context)
        {
            var entity = context.Set<SettingEntity>();
            var settings = await entity.ToListAsync();
            if (settings.Count ==0)
            {
                var setting = new SettingEntity()
                {
                    MoneyToPoint = 1000000,
                    PointToMoney = 10000,
                    CompanyEmail = "mcl@michelin.vn",
                    CompanyAddress = "123 Phạm Văn Đồng,P.13, Q.Bình Thạnh, Tp. Hồ Chí Minh",
                    CompanyFax = "08.5556677",
                    CompanyName = "Công ty TNHH TMDV Michelin",
                    CompanyPhone = "08.5556677",
                    CompanyWebsite = "michelin.vn",
                    OldTruckTireWarmingDays = 100,
                    OldProductWarmingDays = 100,
                    OldTravelTireWarmingDays = 100,
                    IsAllowNegativeInventoryBill = true,
                    IsOnlyDestroyBillOnSameDay = true,
                    CompanyDescription = "Trung tâm vỏ xe ô tô -cân chỉnh thước lại - phụ tùng ô tô",
                    IsAllowUsingPoint = false,
                    CanSaleWithDebtForRetailBill = true
                };
                await entity.AddAsync(setting);
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateDefaultUnits(DatabaseContext context)
        {

            List<string> unitNames = new List<string>()
                {
                    "Cái", "Phuy", "Lít"
                };
            foreach (string name in unitNames)
            {
                var currentUnit = await context.Units.SingleOrDefaultAsync(p => p.Name == name);
                if (currentUnit == null)
                {
                    var unit = new UnitEntity()
                    {
                        Name = name
                    };
                    await context.Units.AddAsync(unit);
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task AddDefaultUsers(RoleManager<RoleEntity> roleManager,
           UserManager<UserEntity> userManager,
           UserEntity user, string password, DatabaseContext databaseContext)
        {
            var currentUser = await userManager.FindByNameAsync(user.UserName);
            if (currentUser == null)
            {
                await userManager.CreateAsync(user, password);

                // Set default user has OWNER role
                await userManager.AddToRoleAsync(user, "OWNER");

                await userManager.UpdateAsync(user);
            }

        }


        private static async Task SetOwnerHasAllStorages(string userName, DatabaseContext context)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
            var storages = await context.Storages.ToArrayAsync();
            foreach (var storage in storages)
            {
                var currentUserStorage = await context.UserStorages.SingleOrDefaultAsync(us => us.UserId == user.Id && us.StorageId == storage.Id);
                if (currentUserStorage == null)
                {
                    var newUserStorage = new UserStorageEntity
                    {
                        UserId = user.Id,
                        StorageId = storage.Id
                    };
                    await context.UserStorages.AddAsync(newUserStorage);

                };
            }
            await context.SaveChangesAsync();
        }
    }
}

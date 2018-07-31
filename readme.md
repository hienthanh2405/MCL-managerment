# There 2 environment: 
Development
Production
To change environment, right click project and choice Properties. Change ASPNETCORE_ENVIRONMENT and must restart Visual Studio.

# To clone all function of a entity: 
1. Config mapping repository on Startup.cs. example:  
services.AddTransient<IProviderRepository, ProviderRepository>();

2. Config Auto Mapper on MappingProfile.cs. example: 
 CreateMap<ProviderEntity, ProviderDto>();

 # Change database history: 
  
  ## beta v.0.1 
  Create new all tables
  Deploy at : 2018-01-02
  
  ## beta v.0.2
  AspNetUsers: Add new fields: IsActive - bit - not null - default is ((1))
  Pages: Create new Page table (Id, Index, Name, ValidRoleNames)
  Deploy at: 15:06 2018-01-10

  ##beta v.0.3 
  Product: Add new fields: Code, ExpiryWeekYear, ExpiryDate, WholeSalePrice.
  UserStorage: Create new table : (Id , UserId, StorageId)
  ProductStorage: Add new field: PlacePosition
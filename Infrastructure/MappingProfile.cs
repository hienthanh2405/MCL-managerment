using API.Controllers;
using API.Entities;
using API.Models;
using AutoMapper;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace API.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProviderEntity, ProviderDto>();
            CreateMap<UnitEntity, UnitDto>();
            CreateMap<ProductCategoryEntity, ProductCategoryDto>();
            CreateMap<SupplierEntity, SupplierDto>();
            CreateMap<ProductEntity, ProductDto>().ForMember( x => x.UnitConverterList , y=> y.Ignore());
            CreateMap<StorageEntity, StorageDto>();
            CreateMap<ProductStorageEntity, ProductStorageDto>().ForMember(x => x.ProductProductionDateList, y => y.Ignore() );
            CreateMap<WarehousingEntity, WarehousingDto>().ForMember(x => x.SupllierBillList, y => y.Ignore()).
               ForMember(x => x.ProductList, y => y.Ignore());
            CreateMap<BillEntity, BillDto>().ForMember(x => x.ProductList, y => y.Ignore());
            CreateMap<BillEntity, BillForGetSingleDto>().ForMember(x => x.ProductList, y => y.Ignore());
            CreateMap<SettingEntity, SettingDto>();
            CreateMap<UserEntity, UserDto>();
            CreateMap<RoleEntity, RoleDto>();
            CreateMap<PageEntity, PageDto>();
            CreateMap<UserStorageEntity, UserStorageDto>();
            CreateMap<CustomerEntity, CustomerDto>();
            CreateMap<InventoryEntity, InventoryDto>();
            CreateMap<ProductForCreationDto, ProductForCreateImportDto>().ForMember(x => x.ProductProductionDateList, y => y.Ignore());
            CreateMap<CapitalPriceTrackingDto, CapitalPriceTrackingDetailDto>();
        }
    }
}

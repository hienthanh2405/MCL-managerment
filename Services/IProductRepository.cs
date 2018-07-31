using API.Controllers;
using API.Entities;
using API.Infrastructure;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProductRepository : ICodeGenericRepository<ProductEntity, ProductDto , ProductForCreationDto>
    {
        Task<ProductPagedResultDto> GetListAsync(int offset, int limit, string keyword,
            Guid currentStorageId,
            SortOptions<ProductDto, ProductEntity> sortOptions,
            FilterOptions<ProductDto, ProductEntity> filterOptions,
            bool isOldProduct
            );

        new Task<Guid> CreateAsync(ProductForCreationDto productForCreationDto);
        new Task<Guid> EditAsync(Guid id , ProductForCreationDto updationDto);

        Task<ProductImportForReturnDto> ImportAsync(string productList);

        Task<bool> ImportToCreateAsync(List<ProductForCreateImportDto> productList);

        Task<bool> IsDuplicatedName(Guid id, string name);

        Task<PagedResults<ProductDto>> GetProductListByCategoryIdsAsync(List<Guid> productCategoryIds);
    }

}

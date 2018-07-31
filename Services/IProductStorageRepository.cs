using API.Entities;
using API.Infrastructure;
using API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProductStorageRepository : IGenericRepository<ProductStorageEntity, ProductStorageDto , ProductStorageForCreationDto>
    {
        Task<PagedResults<ProductStorageDto>> GetListAsync(int offset, int limit, string keyword,
            SortOptions<ProductStorageDto, ProductStorageEntity> sortOptions,
            FilterOptions<ProductStorageDto, ProductStorageEntity> filterOptions,
            Guid storageId,
            string inventoryStatus = "ALL"
        );

        new Task<Guid> CreateAsync(ProductStorageForCreationDto productStorageForCreationDto);
        new Task<Guid> EditAsync(Guid id, ProductStorageForCreationDto productStorageForCreationDto);
        Task<ProductStorageDto> GetByStorageIdAndProductIdAsync(Guid storageId, Guid productId);
        Task<ProductStorageReportDto> GetReport(string inventoryStatus, ICollection<Guid> storageIds);

        Task<bool> FixAllCapitalPriceTracking();

        Task<List<CapitalPriceTrackingDetailDto>> GetCapitalPriceHistory(Guid productStorageId);

         Task<List<CapitalPriceTrackingDetailDto>> ChangeInitialCapitalPriceTracking(Guid productStorageId, CapitalPriceTrackingDto updatedDto);
    }
}

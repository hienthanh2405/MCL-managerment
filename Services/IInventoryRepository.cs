using API.Entities;
using API.Infrastructure;
using API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IInventoryRepository : ICodeGenericRepository<InventoryEntity, InventoryDto , InventoryForCreationDto>
    {
        Task<PagedResults<InventoryDto>> GetListAsync(
            int offset, int limit, string keyword,
            ICollection<Guid> storageIds,
            SortOptions<InventoryDto, InventoryEntity> sortOptions,
            FilterOptions<InventoryDto, InventoryEntity> filterOptions
        );

        new Task<Guid> CreateAsync(InventoryForCreationDto inventoryForCreation);
        new Task<Guid> EditAsync(Guid id, InventoryForCreationDto inventoryForCreation);
        Task<Guid> DestroyAsync(Guid id);
        Task<Guid> BalanceAsync(Guid id);

        Task<InventoryReportDto> GetReport(DateTime FromDate, DateTime ToDate, ICollection<Guid> StorageIds);
    }
}

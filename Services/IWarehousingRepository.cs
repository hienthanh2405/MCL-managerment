using API.Entities;
using API.Infrastructure;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IWarehousingRepository : IGenericRepository<WarehousingEntity, WarehousingDto , WarehousingForCreationDto>
    {
        Task<PagedResults<WarehousingDto>> GetListAsync(
            int offset, int limit, string keyword,
            ICollection<Guid> storageIds,
            SortOptions<WarehousingDto, WarehousingEntity> sortOptions,
            FilterOptions<WarehousingDto, WarehousingEntity> filterOptions,
             DateTime fromDate, DateTime toDate
            );

        new Task<WarehousingDto> GetSingleAsync(Guid id);

        new Task<Guid> CreateAsync(WarehousingForCreationDto productStorageForCreationDto);
        new Task<Guid> EditAsync(Guid id , WarehousingForCreationDto creationDto);

        Task<bool> IsDuplicatedCode(string code);
        Task<Guid> DestroyAsync(Guid id);
        Task<WarehousingReportDto> GetReport(DateTime fromDate, DateTime toDate, ICollection<Guid> storageIds);
        Task<bool> IsDuplicatedSupplierBillCode(string code);

        Task<List<WarehousingHistoryOfProductDto>> GetWarehousingHistory(Guid productId, Guid storageId, DateTime fromDate, DateTime toDate);
    }
}

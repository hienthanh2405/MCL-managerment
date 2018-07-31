using API.Entities;
using API.Infrastructure;
using API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IBillRepository : IGenericRepository<BillEntity, BillDto , BillForCreationDto>
    {
        Task<PagedResults<BillDto>> GetListAsync(int offset, int limit, string keyword, SortOptions<BillDto, BillEntity> sortOptions, FilterOptions<BillDto, BillEntity> filterOptions
            , List<Guid> storageIds, DateTime fromDate, DateTime toDate);
        new Task<BillForGetSingleDto> GetSingleAsync(Guid id);
        new Task<Guid> CreateAsync(BillForCreationDto billForCreationDto);
        new Task<Guid> EditAsync(Guid id, BillForCreationDto billForCreationDto);
        Task<Guid> DestroyAsync(Guid id);
        Task<BillReportDto> GetReport(DateTime FromDate, DateTime ToDate, ICollection<Guid> StorageIds);

        Task<List<BillHistoryOfProductDto>> GetBillHistory(Guid productId, Guid storageId, DateTime fromDate, DateTime toDate);
    }
}

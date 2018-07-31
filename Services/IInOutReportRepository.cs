using API.Infrastructure;
using API.Models;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IInOutReportRepository
    {
        Task<PagedResults<InOutReportDto>> GetInOutReportListAsync(int offset, int limit,
            string keyword, Guid storageId, DateTime fromDate, DateTime toDate);

        Task<PagedResults<InOutReportDto>> GetInOutReportAllAsync(Guid storageId,
            DateTime fromDate, DateTime toDate);
    }
}

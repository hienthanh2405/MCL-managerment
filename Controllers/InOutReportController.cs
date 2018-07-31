using API.Entities;
using API.Infrastructure;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("InOutReports")]
    [Authorize]
    public class InOutReportController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IInOutReportRepository _repository;

        public InOutReportController( DatabaseContext context , IInOutReportRepository repository)    
        {
            _repository = repository;
            _context = context;   
        }

        public async Task<IActionResult> GetListAsync(
            [FromQuery] int offset,
            [FromQuery] int limit,
            [FromQuery] string keyword,
            [FromQuery] Guid storageId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate
            )
        {
            
            var handledData = await _repository.GetInOutReportListAsync(offset, limit, keyword,
                storageId, fromDate, toDate
                );

            var items = handledData.Items.ToArray();

            int totalSize = handledData.TotalSize;

            return Ok(new  GetListResponse(items, totalSize) );
        }

        [Route("all")]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] Guid storageId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate
            )
        {

            var handledData = await _repository.GetInOutReportAllAsync(storageId, fromDate, toDate);

            var items = handledData.Items.ToArray();

            int totalSize = handledData.TotalSize;

            return Ok(new GetListResponse(items, totalSize));
        }
    }
}

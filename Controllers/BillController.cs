using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("Bills")]
    [Authorize]
    public class BillController : GenericController<BillEntity, BillDto, BillForCreationDto>
    {
        private readonly IBillRepository _billRepository;
        private readonly DatabaseContext _context;
        private readonly DbSet<BillEntity> _entity;

        public BillController(IBillRepository billRepository, DatabaseContext context)
            : base(billRepository, context)
        {
            _billRepository = billRepository;
            _context = context;
            _entity = _context.Set<BillEntity>();
        }

        public async Task<IActionResult> GetBillsAsync(
             [FromQuery] int offset,
             [FromQuery] int limit,
             [FromQuery] string keyword,
             [FromQuery] SortOptions<BillDto, BillEntity> sortOptions,
             [FromQuery] FilterOptions<BillDto, BillEntity> filterOptions,
             [FromQuery] List<Guid> storageIds,
             [FromQuery] DateTime fromDate,
             [FromQuery] DateTime toDate
             )
        {

            var handledData = await _billRepository.GetListAsync(offset, limit, keyword, sortOptions, filterOptions , storageIds , fromDate, toDate);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new { data = items, totalSize });
        }

        [HttpGet("{id}")]
        public async override Task<IActionResult> GetEntityAsync(Guid id)
        {
            try
            {
                var handledData = await _billRepository.GetSingleAsync(id);
                return Ok(handledData);
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
        }


        [Route("report")]
        [HttpGet]
        public async Task<IActionResult> GetBillReportAsync( DateTime fromDate, DateTime toDate, ICollection<Guid> storageIds)
        {
            var data = await _billRepository.GetReport(fromDate, toDate, storageIds);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async override Task<IActionResult> DeleteEntityAsync(Guid id)
        {
            await Task.Run(() => { });
            return BadRequest(new ExceptionResponse("Bill is banned for for deletting.", "backend_error_banned_deleting"));
        }
        [HttpPut("destroy/{id}")]
        public async Task<IActionResult> ChangeStatusAsync(Guid id)
        {
            try
            {
                var returnId = await _billRepository.DestroyAsync(id);
                return Ok(new { id = returnId });
            }
            catch (Exception ex)
            {
                if (ex.Message == "inventory_less_than_input_amount")
                {
                    return BadRequest(new ExceptionResponse("InventoryException", "inventory_less_than_input_amount"));
                }
                return BadRequest(new ExceptionResponse(ex.Message));
            }

        }

        [Route("billHistory")]
        [HttpGet]
        public async Task<IActionResult> GetBillHistory([FromQuery] Guid productId, [FromQuery] Guid storageId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var data = await _billRepository.GetBillHistory(productId, storageId , fromDate, toDate);
            return Ok(data);
        }

      

    }
}

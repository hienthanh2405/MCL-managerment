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
    [Route("Inventories")]
    [Authorize]
    public class InventoryController : CodeGenericController<InventoryEntity, InventoryDto, InventoryForCreationDto>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly DatabaseContext _context;
        private readonly DbSet<InventoryEntity> _entity;

        public InventoryController(IInventoryRepository billRepository, DatabaseContext context)
            : base(billRepository, context)
        {
            _inventoryRepository = billRepository;
            _context = context;
            _entity = _context.Set<InventoryEntity>();
        }

        public async Task<IActionResult> GetInventorysAsync(
             [FromQuery] int offset,
             [FromQuery] int limit,
             [FromQuery] string keyword,
             [FromQuery] ICollection<Guid> storageIds,
             [FromQuery] SortOptions<InventoryDto, InventoryEntity> sortOptions,
             [FromQuery] FilterOptions<InventoryDto, InventoryEntity> filterOptions)
        {
            var handledData = await _inventoryRepository.GetListAsync(offset, limit, keyword, storageIds, sortOptions, filterOptions);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new GetListResponse(items, totalSize));
        }

        [Route("report")]
        [HttpGet]
        public async Task<IActionResult> GetInventoryReportAsync(DateTime fromDate, DateTime toDate, ICollection<Guid> storageIds)
        {
            var data = await _inventoryRepository.GetReport(fromDate, toDate, storageIds);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async override Task<IActionResult> DeleteEntityAsync(Guid id)
        {
            await Task.Run(() => { });
            return BadRequest(new ExceptionResponse("Inventory is banned for for deleting.", "backend_error_banned_deleting"));
        }

        [HttpPut("{id}")]
        public async override Task<IActionResult> UpdateEntityAsync(Guid id , [FromBody] InventoryForCreationDto creationDto)
        {
            await Task.Run(() => { });
            return BadRequest(new ExceptionResponse("Inventory is banned for for editting.", "backend_error_banned_deleting"));
        }

        [HttpPut("balance/{id}")]
        public async Task<IActionResult>BalanceAsync(Guid id)
        {
            try
            {
                var returnId = await _inventoryRepository.BalanceAsync(id);
                return Ok(new { id = returnId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message, ex.Message));
            }
        }

        [HttpPut("destroy/{id}")]
        public async Task<IActionResult> DestroyAsync(Guid id)
        {
            try
            {
                var returnId = await _inventoryRepository.DestroyAsync(id);
                return Ok(new { id = returnId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message , ex.Message));
            }
        }

    }
}

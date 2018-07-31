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
    [Route("Warehousings")]
    [Authorize]
    public class WarehousingController : GenericController<WarehousingEntity, WarehousingDto, WarehousingForCreationDto>
    {
        private readonly IWarehousingRepository _repository;
        private readonly DatabaseContext _context;
        private readonly DbSet<WarehousingEntity> _entity;

        public WarehousingController(IWarehousingRepository genericRepository,
            DatabaseContext context) : base(genericRepository, context)
        {
            _repository = genericRepository;
            _context = context;
            _entity = _context.Set<WarehousingEntity>();
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] int offset,
            [FromQuery] int limit,
            [FromQuery] string keyword,
            [FromQuery] ICollection<Guid> storageIds,
            [FromQuery] SortOptions<WarehousingDto, WarehousingEntity> sortOptions,
            [FromQuery] FilterOptions<WarehousingDto, WarehousingEntity> filterOptions,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate
            )
        {

            var handledData = await _repository.GetListAsync(offset, limit, keyword, storageIds, 
                sortOptions, filterOptions, 
                fromDate, toDate);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new GetListResponse(items, totalSize));
        }

        [HttpGet ("duplicated/{code}")]
        public async Task<IActionResult> IsDuplicatedCodeAsync(string code)
        {
            var isDuplicated = await _repository.IsDuplicatedCode(code);
            return Ok( new DuplicatedCheckReturnDto(isDuplicated) );
        }

        [HttpGet("dupplicatedSupplierBill/{code}")]
        public async Task<IActionResult> IsDuplicatedSupplierBillCodeAsync(string code)
        {
            var isDuplicated = await _repository.IsDuplicatedSupplierBillCode(code);
            return Ok(new DuplicatedCheckReturnDto(isDuplicated));
        }

        [HttpPut("destroy/{id}")]
        public async Task<IActionResult> ChangeStatusAsync(Guid id)
        {
            try
            {
                var returnId = await _repository.DestroyAsync(id);
                return Ok(new { id = returnId });
            }
            catch (Exception ex)
            {
                if(ex.Message == "inventory_less_than_input_amount")
                {
                    return BadRequest( new ExceptionResponse("InventoryException", "inventory_less_than_input_amount"));
                }
                return BadRequest(new ExceptionResponse(ex.Message));
            }

        }

        [HttpDelete("{id}")]
        public async override Task<IActionResult> DeleteEntityAsync(Guid id)
        {
            await Task.Run(() =>{});

            return BadRequest( new ExceptionResponse( "Warehousing is banned for deletion"));
        }
        [Route("report")]
        [HttpGet]
        public async Task<IActionResult> GetReportAsync(DateTime fromDate, DateTime toDate, ICollection<Guid> storageIds)
        {
            var data = await _repository.GetReport(fromDate, toDate, storageIds);
            return Ok(data);
        }

        [Route("warehousingHistory")]
        [HttpGet]
        public async Task<IActionResult> GetWarehousingHistory([FromQuery] Guid productId, [FromQuery] Guid storageId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var data = await _repository.GetWarehousingHistory(productId, storageId, fromDate, toDate);
            return Ok(data);
        }
    }
}

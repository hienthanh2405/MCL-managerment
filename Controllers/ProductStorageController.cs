using API.Entities;
using API.Helpers;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("ProductStorages")]
    [Authorize]
    public class ProductStorageController : GenericController<ProductStorageEntity, ProductStorageDto, ProductStorageForCreationDto>
    {
        private readonly DatabaseContext _context;
        private readonly IProductStorageRepository _productStorageRepository;
        private readonly DbSet<ProductStorageEntity> _entity;
        
        public ProductStorageController(IProductStorageRepository
            productStorageRepository, DatabaseContext context) : base(productStorageRepository, context)
        {
            _productStorageRepository = productStorageRepository;
            _context = context;
            _entity = _context.Set<ProductStorageEntity>();

        }
        public async Task<IActionResult> GeListAsync(
         [FromQuery] int offset,
         [FromQuery] int limit,
         [FromQuery] SortOptions<ProductStorageDto, ProductStorageEntity> sortOptions,
         [FromQuery] FilterOptions<ProductStorageDto, ProductStorageEntity> ProductStorageOptions,
         [FromQuery] string keyword,
         [FromQuery] Guid storageId ,
         [FromQuery] string inventoryStatus = "ALL"
         )
        {
            var handledData = await _productStorageRepository.GetListAsync(offset, limit, keyword, sortOptions, ProductStorageOptions, storageId, inventoryStatus);
            var items = handledData.Items;
          
            int totalSize = handledData.TotalSize;
            return Ok(new { data = items, totalSize });
        }

        [HttpGet("{storageId}/{productId}")]
        public async Task<IActionResult> GetEntityAsync(Guid storageId , Guid productId)
        {
            try
            {
                var handledData = await _productStorageRepository.GetByStorageIdAndProductIdAsync(storageId , productId);
                return Ok(handledData);
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
        }

        [HttpPost]
        public override async Task<IActionResult> CreateEntityAsync([FromBody] ProductStorageForCreationDto creationDto)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var ProductId = await _productStorageRepository.CreateAsync(creationDto);
                return Created("", new { id = ProductId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message, "backend_error_dupplicated"));
            }
        }

        [HttpPut("{id}")]
        public async override Task<IActionResult> UpdateEntityAsync(Guid id, [FromBody] ProductStorageForCreationDto creationDto)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var ProductId = await _productStorageRepository.EditAsync(id, creationDto);
                return Ok(new { id = ProductId });

            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }

        }

        [HttpDelete("{id}")]
        public async override Task<IActionResult> DeleteEntityAsync(Guid id)
        {
            await Task.Run(() => { });
            return BadRequest(new ExceptionResponse("Product Storage is banned for for deletting.", "backend_error_banned_deleting"));
        }

        [Route("report")]
        [HttpGet]
        public async Task<IActionResult> GetReportAsync( string inventoryStatus, ICollection<Guid> storageIds)
        {
            var data = await _productStorageRepository.GetReport(inventoryStatus, storageIds);
            return Ok(data);
        }

        [Route("fix")]
        [HttpGet]
        public async Task<IActionResult> FixAllCapitalPrice()
        {
            var data = await _productStorageRepository.FixAllCapitalPriceTracking();
            return Ok(data);
        }

        [Route("history/{productStorageId}")]
        [HttpGet]
        public async Task<IActionResult> GetCapitalPriceHistory(Guid productStorageId)
        {
            var list = await _productStorageRepository.GetCapitalPriceHistory(productStorageId);
            return Ok( new {data = list });
        }


        [HttpPut("InitialCapitalPriceTracking/{id}")]
        public async Task<IActionResult> ChangeInitialCapitalPriceTracking(Guid id, [FromBody] CapitalPriceTrackingDto updationDto)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var capitalPriceTrackingDetails = await _productStorageRepository.ChangeInitialCapitalPriceTracking(id, updationDto);
                return Ok(new { CapitalPriceTrackingDetails = capitalPriceTrackingDetails });

            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }

        }

     
    }
}

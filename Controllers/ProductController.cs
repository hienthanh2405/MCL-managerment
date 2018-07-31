using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("Products")]
    [Authorize]
    public class ProductController : CodeGenericController<ProductEntity, ProductDto, ProductForCreationDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly DatabaseContext _context;

        public ProductController(IProductRepository genericRepository, DatabaseContext context) : base(genericRepository, context)
        {
            _productRepository = genericRepository;
            _context = context;
        }

      
        public async Task<IActionResult> GetProductsAsync(
            [FromQuery] int offset,
            [FromQuery] int limit,
            [FromQuery] string keyword,
            [FromQuery] Guid currentStorageId,
            [FromQuery] SortOptions<ProductDto, ProductEntity> sortOptions,
            [FromQuery] FilterOptions<ProductDto, ProductEntity> ProductOptions,
            [FromQuery] bool isOldProduct = false
            )
        {
            try
            {
            
                var handledData = await _productRepository.GetListAsync(offset, limit, keyword, currentStorageId, sortOptions, ProductOptions, isOldProduct);

                var items = handledData.Items;
                int totalSize = handledData.TotalSize;

                return Ok(new {
                    Data = items,
                    TotalSize =totalSize,
                    OldProductNumber = handledData.OldProductNumber  } );

            } catch(Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
          
        }
        [HttpPost]
       [Route("import")]
        public async Task<IActionResult> ImportAsync([FromBody] ProductImportForRequestDto productImportForRequestDto )
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var result = await _productRepository.ImportAsync(productImportForRequestDto.ProductList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message, ex.Message));
            }
        }

        [HttpPost]
        [Route("ImportCreate")]
        public async Task<IActionResult> ImportCreateAsync([FromBody] ProductListForImportCreateDto creationDto)
        {

            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var result = await _productRepository.ImportToCreateAsync(creationDto.ProductList);
                return Ok( new { IsFinished = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message, ex.Message));
            }
        }

        [HttpGet("duplicatedName/{id}/{name}")]
        public async Task<IActionResult> IsDuplicatedNameCodeAsync(Guid id, string name)
        {
            var isDuplicated = await _productRepository.IsDuplicatedName(id, name);
            return Ok(new DuplicatedCheckReturnDto(isDuplicated));
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetProductListByCategoryAsync(
           [FromQuery] List<Guid> productCategoryIds)
        {
            var response = await _productRepository.GetProductListByCategoryIdsAsync(productCategoryIds);
            return Ok( new GetListResponse (response.Items.ToArray(), response.TotalSize));
        }
    }


    public class ProductListForImportCreateDto
    {
       public List<ProductForCreateImportDto> ProductList { get; set; }
       
    }
    public class ProductForCreateImportDto : ProductForCreationDto
    {
        public string ProductCategoryName { get; set; }
        public string CurrentStorageName { get; set; }
        public string Dot { get; set; }

    }
}

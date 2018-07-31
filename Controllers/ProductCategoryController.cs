using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("ProductCategories")]
    [Authorize]
    public class ProductCategoryController : GenericController<ProductCategoryEntity, ProductCategoryDto, ProductCategoryForCreationDto>
    {
        private readonly DatabaseContext _context;
        private readonly IGenericRepository<ProductCategoryEntity, ProductCategoryDto, ProductCategoryForCreationDto> _genericRepository;

        public ProductCategoryController(IGenericRepository<ProductCategoryEntity, ProductCategoryDto, ProductCategoryForCreationDto>
            genericRepository, DatabaseContext context) : base(genericRepository, context)
        {
            _genericRepository = genericRepository;
            _context = context;
        }
        public async Task<IActionResult> GetProductCategoriesAsync(
         [FromQuery] int offset,
         [FromQuery] int limit,
         [FromQuery] string keyword,
         [FromQuery] SortOptions<ProductCategoryDto, ProductCategoryEntity> sortOptions,
         [FromQuery] FilterOptions<ProductCategoryDto, ProductCategoryEntity> ProductCategoryOptions)
        {
            IQueryable<ProductCategoryEntity> querySearch = _context.ProductCategories.Where(x => x.Name.Contains(keyword));
            var handledData = await _genericRepository.GetListAsync(offset, limit, keyword, sortOptions, ProductCategoryOptions, querySearch);
            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;
            return Ok(new { data = items, totalSize });
        }

    }
}

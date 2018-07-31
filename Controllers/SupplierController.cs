using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("suppliers")]
    [Authorize]
    public class SupplierController : GenericController<SupplierEntity, SupplierDto, SupplierDto>
    {
        private readonly IGenericRepository<SupplierEntity , SupplierDto , SupplierDto> _genericRepository;
        private readonly DatabaseContext _context;

        public SupplierController(IGenericRepository<SupplierEntity, SupplierDto , SupplierDto>
            genericRepository, DatabaseContext context) : base(genericRepository ,context)
        {
            _genericRepository = genericRepository;
            _context = context;
        }

        public async Task<IActionResult> GetSuppliersAsync(
             [FromQuery] int offset,
             [FromQuery] int limit, 
             [FromQuery] string keyword,
             [FromQuery] SortOptions<SupplierDto, SupplierEntity> sortOptions,
             [FromQuery] FilterOptions<SupplierDto, SupplierEntity> supplierOptions)
        {
          
            IQueryable<SupplierEntity> querySearch = _context.Suppliers.Where(x => x.Name.Contains(keyword)
                 || x.Code.Contains(keyword)
                 || x.TaxCode.Contains(keyword)
                 || x.Email.Contains(keyword)
                 || x.Address.Contains(keyword)
                 || x.Phone.Contains(keyword));

            var handledData = await _genericRepository.GetListAsync(offset, limit, keyword, sortOptions , supplierOptions , querySearch);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok( new { data =  items , totalSize });
        }
    }
}

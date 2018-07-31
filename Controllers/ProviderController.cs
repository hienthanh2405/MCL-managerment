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
    [Route("providers")]
    [Authorize]
    public class ProviderController : GenericController<ProviderEntity, ProviderDto, ProviderDto>
    {
        private readonly IGenericRepository<ProviderEntity , ProviderDto , ProviderDto> _genericRepository;
        private readonly DatabaseContext _context;

        public ProviderController(IGenericRepository<ProviderEntity, ProviderDto , ProviderDto > 
            genericRepository, DatabaseContext context) : base(genericRepository, context)
        {
            _genericRepository = genericRepository;
            _context = context;
        }

        public async Task<IActionResult> GetProvidersAsync(
             [FromQuery] int offset,
             [FromQuery] int limit, 
             [FromQuery] string keyword,
             [FromQuery] SortOptions<ProviderDto, ProviderEntity> sortOptions,
             [FromQuery] FilterOptions<ProviderDto, ProviderEntity> providerOptions)
        {

            IQueryable<ProviderEntity> querySearch = _context.Providers.Where(x => 
                x.Name.Contains(keyword)
            || x.Description.Contains(keyword)
            );

            var handledData = await _genericRepository.GetListAsync(offset, limit, keyword, sortOptions , providerOptions , querySearch);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok( new GetListResponse(items, totalSize) );
        }
    }
}

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
    [Route("storages")]
    [Authorize]
    public class StorageController : GenericController<StorageEntity, StorageDto, StorageDto>
    {
        private readonly IStorageRepository _storageRepository;
        private readonly DatabaseContext _context;

        public StorageController(IStorageRepository
            storageRepository, DatabaseContext context) : base(storageRepository, context)
        {
            _storageRepository = storageRepository;
            _context = context;
        }

        public async Task<IActionResult> GetStoragesAsync(
             [FromQuery] int offset,
             [FromQuery] int limit, 
             [FromQuery] string keyword,
             [FromQuery] SortOptions<StorageDto, StorageEntity> sortOptions,
             [FromQuery] FilterOptions<StorageDto, StorageEntity> providerOptions)
        {

            IQueryable<StorageEntity> querySearch = _context.Storages.Where(x => 
                x.Name.Contains(keyword)
            || x.Address.Contains(keyword)
            );

            var handledData = await _storageRepository.GetListAsync(offset, limit, keyword, sortOptions , providerOptions , querySearch);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok( new GetListResponse(items, totalSize) );
        }
    }
}

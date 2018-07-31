using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("Settings")]
    [Authorize]
    public class SettingController : GenericController<SettingEntity, SettingDto, SettingDto>
    {
        private readonly IGenericRepository<SettingEntity, SettingDto , SettingDto> _genericRepository;
        private readonly DatabaseContext _context;
        private readonly DbSet<SettingEntity> _entity;

        public SettingController(IGenericRepository<SettingEntity, SettingDto , SettingDto> genericRepository , DatabaseContext context)
            :base(genericRepository, context)
        {
            _genericRepository = genericRepository;
            _context = context;
            _entity = _context.Set<SettingEntity>();
        }

        public async Task<IActionResult> GetSettingsAsync(
             [FromQuery] int offset,
             [FromQuery] int limit,
             [FromQuery] string keyword,
             [FromQuery] SortOptions<SettingDto, SettingEntity> sortOptions,
             [FromQuery] FilterOptions<SettingDto, SettingEntity> filterOptions)
        {
            IQueryable<SettingEntity> querySearch = _entity;
            
            var handledData = await _genericRepository.GetListAsync(offset, limit, keyword, sortOptions, filterOptions, querySearch);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new { data = items, totalSize });
        }


    }
}

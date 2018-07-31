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
    [Route("Units")]
    [Authorize]
    public class UnitController : GenericController<UnitEntity, UnitDto, UnitDto>
    {
        private readonly IGenericRepository<UnitEntity, UnitDto , UnitDto> _unitRepository;
        private readonly DatabaseContext _context;
      

        public UnitController(IGenericRepository<UnitEntity, UnitDto , UnitDto> unitGenericRepository , DatabaseContext context)
            :base(unitGenericRepository , context)
        {
            _unitRepository = unitGenericRepository;
            _context = context;
        }

        public async Task<IActionResult> GetUnitsAsync(
             [FromQuery] int offset,
             [FromQuery] int limit,
             [FromQuery] string keyword,
             [FromQuery] SortOptions<UnitDto, UnitEntity> sortOptions,
             [FromQuery] FilterOptions<UnitDto, UnitEntity> filterOptions)
        {
            IQueryable<UnitEntity> querySearch = _context.Units.Where(x => x.Name.Contains(keyword));
            
            var handledData = await _unitRepository.GetListAsync(offset, limit, keyword, sortOptions, filterOptions, querySearch);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new { data = items, totalSize });
        }


    }
}

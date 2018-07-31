using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("Pages")]
    [Authorize]
    public class PageController : GenericController<PageEntity, PageDto, PageDto>
    {
        private readonly IPageRepository _pageRepository;
        private readonly DatabaseContext _context;
      

        public PageController(IPageRepository pageGenericRepository , DatabaseContext context)
            :base(pageGenericRepository , context)
        {
            _pageRepository = pageGenericRepository;
            _context = context;
        }

        public async Task<IActionResult> GetPagesAsync(
             [FromQuery] int offset,
             [FromQuery] int limit,
             [FromQuery] string keyword,
             [FromQuery] SortOptions<PageDto, PageEntity> sortOptions,
             [FromQuery] FilterOptions<PageDto, PageEntity> filterOptions)
        {
            IQueryable<PageEntity> querySearch = _context.Pages.Where(x => x.Name.Contains(keyword));
            
            var handledData = await _pageRepository.GetListAsync(offset, limit, keyword, sortOptions, filterOptions, querySearch);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new { data = items, totalSize });
        }

        [HttpGet]
        [Route("roleNames/{pageName}")]
        public async Task<IActionResult> GetRoleNamesByPageName(string pageName)
        {
            var roleNames = await _pageRepository.GetRoleNamesByPageName(pageName);
            return Ok( new { data = roleNames});
        }

        // we can delete this function after 20/4/2018. This is function only for fix data on database.
        [HttpGet]
        [Route("fix")]
        public async Task<IActionResult> FixDuplicatedGenerateRole()
        {
            var pages = _context.Pages.ToList();
            foreach(var page in pages)
            {
                var validRoleNames = JsonConvert.DeserializeObject<List<string>>(page.ValidRoleNames);

                HashSet<string> fixedSet = new HashSet<string>(validRoleNames);
                page.ValidRoleNames = JsonConvert.SerializeObject(fixedSet);
                _context.Pages.Update(page);
                await _context.SaveChangesAsync();
            }
            return Ok();

        }

    }
}

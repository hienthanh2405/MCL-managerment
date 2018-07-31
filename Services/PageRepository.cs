using API.Entities;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Services
{
    public class PageRepository : GenericRepository<PageEntity, PageDto , PageDto> , IPageRepository
    {
        private DatabaseContext _context;

        public PageRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }
        public async Task<String> GetRoleNamesByPageName(string pageName)
        {
            var page = await _context.Pages.SingleOrDefaultAsync(p => p.Name == pageName);
            if (page == null)
            {
                throw new Exception("Can not find page with this name");
            }
            return page.ValidRoleNames;
        }

    }
}

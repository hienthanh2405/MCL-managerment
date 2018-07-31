using API.Entities;
using API.Models;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IPageRepository : IGenericRepository<PageEntity, PageDto , PageDto>
    {
        Task<String> GetRoleNamesByPageName(string pageName);
    }
}

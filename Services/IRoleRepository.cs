using API.Entities;
using API.Infrastructure;
using API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IRoleRepository
    {
        Task<PagedResults<RoleDto>> GetListAsync(int offset, int limit, string keyword,
        SortOptions<RoleDto, RoleEntity> sortOptions, FilterOptions<RoleDto, RoleEntity> filterOptions,
        IQueryable<RoleEntity> querySearch
        );
        Task<PagedResults<RoleDto>> GetAllAsync();
        Task<Guid> CreateAsync( RoleDto creationDto);
        Task<Guid> EditAsync(Guid id, RoleDto creationDto);
        Task<Guid> DeleteAsync(Guid id);
    }
}

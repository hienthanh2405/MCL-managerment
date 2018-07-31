using API.Entities;
using API.Infrastructure;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IUserRepository
    {
        Task<PagedResults<UserDto>> GetListAsync(int offset, int limit, string keyword,
        SortOptions<UserDto, UserEntity> sortOptions, FilterOptions<UserDto, UserEntity> filterOptions,
        IQueryable<UserEntity> querySearch
        );

        Task<Guid> CreateAsync( UserForCreationDto creationDto);

        Task<Guid> UpdateAsync(Guid id, UserForUpdationDto creationDto);

        Task<Guid> ChangeStatus(Guid id, bool status);

        Task<UserDto> GetCurrentUser();

        Task<Guid> ResetPassword(Guid id, UserForResetPasswordDto resetPasswordDto);

        Task<List<UserDto>> GetUsersWithRoleNameAsync(string roleName);
    }
}

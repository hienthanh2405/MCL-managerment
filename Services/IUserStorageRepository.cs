using API.Entities;
using API.Infrastructure;
using API.Models;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IUserStorageRepository : IGenericRepository<UserStorageEntity, UserStorageDto , UserStorageDto>
    {
      Task<PagedResults<UserStorageDto>> GetUserStoragesByUserIdAsync(Guid userId);
    }
}

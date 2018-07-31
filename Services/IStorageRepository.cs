using API.Entities;
using API.Models;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IStorageRepository : IGenericRepository<StorageEntity, StorageDto , StorageDto>
    {
        new Task<Guid> CreateAsync(StorageDto storageDto);
    }

}

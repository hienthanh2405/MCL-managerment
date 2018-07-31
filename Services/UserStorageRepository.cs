using API.Entities;
using API.Infrastructure;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserStorageRepository : GenericRepository<UserStorageEntity, UserStorageDto, UserStorageDto>, IUserStorageRepository
    {
        private DatabaseContext _context;
        private DbSet<UserStorageEntity> _entity;

        public UserStorageRepository(DatabaseContext context) : base(context)
        {
            _context = context;
            _entity = _context.Set<UserStorageEntity>();
        }

        public async Task<PagedResults<UserStorageDto>> GetUserStoragesByUserIdAsync(Guid userId)
        {
          
            var totalSize = await _entity.CountAsync();
            var items = await _entity.ProjectTo<UserStorageDto>()
                .Where( us=> us.UserId == userId).ToArrayAsync();

            return new PagedResults<UserStorageDto>
            {
                Items = items.OrderBy(us => us.Storage.Name),
                TotalSize = totalSize
            };

        }
    }
}

using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("UserStorages")]
    [Authorize]
    public class UserStorageController : GenericController<UserStorageEntity, UserStorageDto, UserStorageDto>
    {
        private readonly DatabaseContext _context;
        private readonly IUserStorageRepository _userStorageRepository;
        private readonly DbSet<UserStorageEntity> _entity;

        public UserStorageController(IUserStorageRepository 
            userStorageRepository, DatabaseContext context) : base(userStorageRepository, context)
        {
            _userStorageRepository = userStorageRepository;
            _context = context;
            _entity = _context.Set<UserStorageEntity>();

        }
        public async Task<IActionResult> GeListAsync(
         [FromQuery] int offset,
         [FromQuery] int limit,
         [FromQuery] SortOptions<UserStorageDto, UserStorageEntity> sortOptions,
         [FromQuery] FilterOptions<UserStorageDto, UserStorageEntity> UserStorageOptions,
         [FromQuery] string keyword,
         [FromQuery] Guid storageId,
         [FromQuery] bool isSoldOut = false
         )
        {
            IQueryable<UserStorageEntity> querySearch = _entity;
            if (keyword != null)
            {
                querySearch = _entity.Where(
                x => x.User.FirstName.Contains(keyword)
                || x.Storage.Name.Contains(keyword)
                );
            }

            var handledData = await _userStorageRepository.GetListAsync(offset, limit, keyword, sortOptions, UserStorageOptions, querySearch);
            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;
            return Ok(new { data = items, totalSize });
        }
        [Route("user/{userId}")]
        public async Task<IActionResult> GetUserStoragesByUserIdAsync(Guid userId)
        {
            var handledData =  await _userStorageRepository.GetUserStoragesByUserIdAsync(userId);
            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;
            return Ok(new { data = items, totalSize });
        }
    }
}

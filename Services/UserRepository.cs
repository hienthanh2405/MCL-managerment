using API.Entities;
using API.Infrastructure;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserRepository : IUserRepository
    {
        private DatabaseContext _context;
        private DbSet<UserEntity> _entity;
        private IMapper _mapper;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
      
        public UserRepository(DatabaseContext context, UserManager<UserEntity> userManager ,
                IHttpContextAccessor httpContextAccessor
         )
        {
            _userManager = userManager;
            _context = context;
            _entity = _context.Set<UserEntity>();
            _httpContextAccessor = httpContextAccessor;

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserEntity, UserDto>();
            });

            _mapper = config.CreateMapper();
        }

        public async Task<PagedResults<UserDto>> GetListAsync(int offset, int limit, string keyword,
           SortOptions<UserDto, UserEntity> sortOptions, FilterOptions<UserDto, UserEntity> filterOptions,
           IQueryable<UserEntity> querySearch
           )
        {
            IQueryable<UserEntity> query = _entity;
            query = sortOptions.Apply(query);
            query = filterOptions.Apply(query);
            if (keyword != null)
            {
                query = querySearch;
            }

            var size = await query.CountAsync();

            var items = await query
                .Skip(offset * limit)
                .Take(limit)    
                .ToArrayAsync();

            List<UserDto> returnUserList = new List<UserDto>();
            
                foreach (UserEntity user in items)
            {
                var roleNames = await _userManager.GetRolesAsync(user);
                var userStorages = _context.UserStorages.Where(us => us.UserId == user.Id).Include(us=>us.Storage).OrderBy(s => s.Storage.Name);
                var storages = new List<StorageDto>();

                foreach(var userStorage in userStorages)
                {
                    storages.Add( Mapper.Map<StorageDto>( userStorage.Storage) );
                }
             
                var userDto = new UserDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Id = user.Id,
                    UserName = user.UserName,
                    JobTitle = user.JobTitle,
                    IsActive = user.IsActive,
                    RoleNames = roleNames,
                    AccessibleStorages = storages
                };
                returnUserList.Add(userDto);
            }
           
            return new PagedResults<UserDto>
            {
                Items = returnUserList,
                TotalSize = size
            };

        }

        public async Task<List<UserDto>> GetUsersWithRoleNameAsync(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            var userDtos = Mapper.Map<List<UserDto>>(users);
            return userDtos;
        }

        public async Task<Guid> CreateAsync(UserForCreationDto creationDto)
        {

            UserEntity newUser = Activator.CreateInstance<UserEntity>();
            newUser.IsActive = true;

            foreach (PropertyInfo propertyInfo in creationDto.GetType().GetProperties())
            {
                if (newUser.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    newUser.GetType().GetProperty(propertyInfo.Name).SetValue(newUser, propertyInfo.GetValue(creationDto, null));
                }

            }
            newUser.UserName = creationDto.UserName;
            await _userManager.CreateAsync(newUser, creationDto.Password);

            foreach (string roleName in creationDto.RoleNames)
            {
                await _userManager.AddToRoleAsync(newUser, roleName);
            }
           
        //    await _userManager.UpdateAsync(newUser);

            foreach(Guid storageId in creationDto.StorageIds)
            {
                var storage = await _context.Storages.SingleOrDefaultAsync(s => s.Id == storageId);
                if(storage == null)
                {
                    throw new Exception("Can not find storage with Id");
                }
                var userStorage = new UserStorageEntity()
                {
                    UserId = newUser.Id,
                    StorageId = storageId
                };
                await _context.UserStorages.AddAsync(userStorage);
                await _context.SaveChangesAsync();
            }


           

            return newUser.Id;
        }

        public async Task<Guid> UpdateAsync(Guid id, UserForUpdationDto updationDto)
        {
            var user = await _entity.SingleOrDefaultAsync(r => r.Id == id);

            if (user == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }
            foreach (PropertyInfo propertyInfo in updationDto.GetType().GetProperties())
            {
                string key = propertyInfo.Name;
                if (key != "Id" && user.GetType().GetProperty(key) != null)
                {
                    user.GetType().GetProperty(key).SetValue(user, propertyInfo.GetValue(updationDto, null));
                }
            }

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
            await _userManager.AddToRolesAsync(user, updationDto.RoleNames);

            _entity.Update(user);

            var currentUserStorages = await _context.UserStorages.Where(us => us.UserId == user.Id).ToListAsync();
            foreach(var userStorage in currentUserStorages)
            {
                var updatedId = updationDto.StorageIds.SingleOrDefault(s => s == userStorage.StorageId);
                if(updatedId == Guid.Empty)
                {
                    _context.UserStorages.Remove(userStorage);
                }
            }
            foreach (var storageId in updationDto.StorageIds)
            {
                var currentUserStorage = currentUserStorages.SingleOrDefault(us => us.StorageId == storageId);
                if (currentUserStorage == null)
                {
                    var userStorage = new UserStorageEntity()
                    {
                        StorageId = storageId,
                        UserId = user.Id
                    };
                    await _context.AddAsync(userStorage);
                }
            }

            var updated = await _context.SaveChangesAsync();
            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }
            return id;
        }

        public async Task<Guid> ChangeStatus( Guid id, bool status)
        {
            var user = await _entity.FirstOrDefaultAsync(p => p.Id == id);
            if(user == null)
            {
                throw new Exception("Can not find user with this id");
            }
            user.IsActive = status;
            _entity.Update(user);

            var updated = await _context.SaveChangesAsync();
            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }
            return id;

        }

        public async Task<UserDto> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var roleNames = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.RoleNames = roleNames;
            return userDto;
        }

        public async Task<Guid> ResetPassword(Guid id, UserForResetPasswordDto resetPasswordDto)
        {
            var user = await _entity.SingleOrDefaultAsync(r => r.Id == id);

            if (user == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordDto.Password); 
  
            _entity.Update(user);
            var updated = await _context.SaveChangesAsync();
            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }
            return id;
        }
    }
}

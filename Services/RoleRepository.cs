using API.Entities;
using API.Infrastructure;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Services
{
    public class RoleRepository : IRoleRepository
    {
        private DatabaseContext _context;
        private DbSet<RoleEntity> _entity;
     
        private readonly RoleManager<RoleEntity> _roleManager;


        public RoleRepository(DatabaseContext context, RoleManager<RoleEntity> roleManager)
        {
            _roleManager = roleManager;
            _context = context;
            _entity = _context.Set<RoleEntity>();
        }

        public async Task<PagedResults<RoleDto>> GetListAsync(int offset, int limit, string keyword,
           SortOptions<RoleDto, RoleEntity> sortOptions, FilterOptions<RoleDto, RoleEntity> filterOptions,
           IQueryable<RoleEntity> querySearch
           )
        {
            IQueryable<RoleEntity> query = _entity;
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
                .ProjectTo<RoleDto>()
                .ToArrayAsync();

            List<UserDto> returnUserList = new List<UserDto>();
           
           
            return new PagedResults<RoleDto>
            {
                Items = items,
                TotalSize = size
            };

        }

        public async Task<Guid> CreateAsync(RoleDto creationDto)
        {
            RoleEntity createdRole = Activator.CreateInstance<RoleEntity>();

            foreach (PropertyInfo propertyInfo in creationDto.GetType().GetProperties())
            {
                if (createdRole.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    createdRole.GetType().GetProperty(propertyInfo.Name).SetValue(createdRole, propertyInfo.GetValue(creationDto, null));
                }

            }
          
            await _roleManager.CreateAsync(createdRole);
            return createdRole.Id;
        }

        public async Task<Guid> DeleteAsync(Guid id)
        {
            var entity = await _entity.SingleOrDefaultAsync(r => r.Id == id);
            if (entity == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }

            _entity.Remove(entity);
            var deleted = await _context.SaveChangesAsync();
            if (deleted < 1)
            {
                throw new InvalidOperationException("Database context could not delete data.");
            }
            return id;
        }

        public async Task<Guid> EditAsync(Guid id, RoleDto creationDto)
        {
            var entity = await _entity.SingleOrDefaultAsync(r => r.Id == id);
            if (entity == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }
            foreach (PropertyInfo propertyInfo in creationDto.GetType().GetProperties())
            {
                string key = propertyInfo.Name;
                if (key != "Id" && entity.GetType().GetProperty(key) != null)
                {
                    entity.GetType().GetProperty(key).SetValue(entity, propertyInfo.GetValue(creationDto, null));
                }
            }
            entity.NormalizedName = creationDto.Name.ToUpper();

            _entity.Update(entity);
            var updated = await _context.SaveChangesAsync();
            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }
            return id;
        }

        public async Task<PagedResults<RoleDto>> GetAllAsync()
        {

            IQueryable<RoleEntity> query = _entity;
            var totalSize = await query.CountAsync();

            var items = await query
                .ProjectTo<RoleDto>()
                .ToArrayAsync();

            return new PagedResults<RoleDto>
            {
                Items = items.OrderBy( r=>r.Index),
                TotalSize = totalSize
            };
        }
    }
}

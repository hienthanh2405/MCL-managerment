using API.Entities;
using API.Infrastructure;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Services
{
    public class CodeGenericRepository<TEntity, TDto , TCreationDto> :  GenericRepository<TEntity, TDto, TCreationDto>, ICodeGenericRepository<TEntity, TDto, TCreationDto>
        where TDto : class where TCreationDto : CodeBaseDto where TEntity: CodeBaseEntity 
    {
        private readonly DatabaseContext _context;
        private DbSet<TEntity> _entities;
        string errorMessage = string.Empty;

        public CodeGenericRepository(DatabaseContext context) : base (context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public new async Task<Guid> CreateAsync(TCreationDto creationDto)
        {
            var currentEntity = await _entities.SingleOrDefaultAsync(e => e.Code == creationDto.Code);
            if(currentEntity != null)
            {
                throw new Exception("backend_error_dupplicated_code");
            }

            TEntity newEntity = Activator.CreateInstance<TEntity>();

            foreach (PropertyInfo propertyInfo in creationDto.GetType().GetProperties())
            {
                if (newEntity.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    newEntity.GetType().GetProperty(propertyInfo.Name).SetValue(newEntity, propertyInfo.GetValue(creationDto, null));
                }

            }

            var result = await _entities.AddAsync(newEntity);

            var created = await _context.SaveChangesAsync();
            if (created < 1)
            {
                throw new InvalidOperationException("Database context could not create data.");
            }
            return newEntity.Id;
        }

        public async Task<bool> IsDuplicatedCode(Guid id, string code)
        {
         
            var entity = await _entities.SingleOrDefaultAsync(r => r.Code == code && r.Id != id);
            if (entity == null)
            {
                return false;
            } 
            return true;
        }

        public new async Task<PagedResults<TDto>> GetAllAsync()
        {
            IQueryable<TEntity> query = _entities;
            var totalSize = await query.CountAsync();

            var items = await query.OrderBy( e => e.Code)
                .ProjectTo<TDto>()
                .ToArrayAsync();

            return new PagedResults<TDto>
            {
                Items = items,
                TotalSize = totalSize
            };
        }

    }
}

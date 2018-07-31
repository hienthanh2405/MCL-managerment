using API.Entities;
using API.Infrastructure;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Services
{
    public class GenericRepository<TEntity, TDto , TCreationDto> : IGenericRepository<TEntity, TDto, TCreationDto> 
        where TDto : class where TCreationDto : class where TEntity:BaseEntity
    {
        private readonly DatabaseContext _context;
        private DbSet<TEntity> _entities;
        private IMapper _mapper;
        string errorMessage = string.Empty;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<TEntity, TDto>();
            });

            _mapper = config.CreateMapper();
        }

        public async Task<TDto> GetSingleAsync(Guid id)
        {
            var entity = await _entities.SingleOrDefaultAsync(r => r.Id == id);
            if (entity == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }
            return _mapper.Map<TDto>(entity);      
        }

        public async Task<PagedResults<TDto>> GetListAsync(int offset, int limit, string keyword, 
            SortOptions<TDto, TEntity> sortOptions, FilterOptions<TDto, TEntity> filterOptions,
            IQueryable<TEntity> querySearch
            )
        {
            IQueryable<TEntity> query = _entities;
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
                .ProjectTo<TDto>()
                .ToArrayAsync();

            return new PagedResults<TDto>
            {
                Items = items,
                TotalSize = size
            };

        }
        public async Task<PagedResults<TDto>> GetAllAsync()
        {

            IQueryable<TEntity> query = _entities;
            var totalSize = await query.CountAsync();

            var items = await query
                .ProjectTo<TDto>()
                .ToArrayAsync();

            return new PagedResults<TDto>
            {
                Items = items,
                TotalSize = totalSize
            };
        }

        public async Task<Guid> CreateAsync(TCreationDto creationDto)
        {
          
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

        public async Task<Guid> EditAsync(Guid id, TCreationDto creationDto)
        {
            var entity = await _entities.SingleOrDefaultAsync(r => r.Id == id);
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

            _entities.Update(entity);
            var updated = await _context.SaveChangesAsync();
            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }
            return id;
        }

        public async Task<Guid> DeleteAsync(Guid id)
        {
            var entity = await _entities.SingleOrDefaultAsync(r => r.Id == id);
            if (entity == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }

            _entities.Remove(entity);
            var deleted = await _context.SaveChangesAsync();
            if (deleted < 1)
            {
                throw new InvalidOperationException("Database context could not delete data.");
            }
            return id;
        }
    }
}

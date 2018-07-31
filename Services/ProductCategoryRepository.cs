using API.Entities;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Services
{
    public class ProductCategoryRepository : GenericRepository<ProductCategoryEntity, ProductCategoryDto , ProductCategoryForCreationDto> , IProductCategoryRepository
    {
        private DatabaseContext _context;

        public ProductCategoryRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public new async Task<Guid> CreateAsync(ProductCategoryForCreationDto productCategoryForCreationDto)
        {
            var id = Guid.NewGuid();
            var newEntity = new ProductCategoryEntity {
                Id = id
            };

            foreach (PropertyInfo propertyInfo in productCategoryForCreationDto.GetType().GetProperties())
            {
                if (newEntity.GetType().GetProperty(propertyInfo.Name) != null)
                {
                   newEntity.GetType().GetProperty(propertyInfo.Name).SetValue(newEntity, propertyInfo.GetValue(productCategoryForCreationDto, null));
                }

            }

            await _context.ProductCategories.AddAsync(newEntity);

            var created = await _context.SaveChangesAsync();
            if (created < 1)
            {
                throw new InvalidOperationException("Database context could not create data.");
            }
            return id;
        }

        public new async Task<Guid> EditAsync(Guid id , ProductCategoryForCreationDto ProductCategoryForCreationDto)
        {
            var entity = await _context.ProductCategories.SingleOrDefaultAsync(r => r.Id == id);
            if (entity == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }
            foreach (PropertyInfo propertyInfo in ProductCategoryForCreationDto.GetType().GetProperties())
            {
                string key = propertyInfo.Name;
                if (key != "Id" && entity.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    entity.GetType().GetProperty(key).SetValue(entity, propertyInfo.GetValue(ProductCategoryForCreationDto, null));
                }
            }

            _context.ProductCategories.Update(entity);
            var updated = await _context.SaveChangesAsync();
            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }
            return id;


        }

    }
}

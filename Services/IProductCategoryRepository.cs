using API.Entities;
using API.Infrastructure;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProductCategoryRepository : IGenericRepository<ProductCategoryEntity, ProductCategoryDto , ProductCategoryForCreationDto>
    {
        //Task<Guid> CreateAsync(ProductCategoryForCreationDto productCategoryForCreationDto);
        //Task<Guid> EditAsync(Guid id, ProductCategoryForCreationDto ProductCategoryForCreationDto);
    }
}

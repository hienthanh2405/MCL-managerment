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
    public interface ICodeGenericRepository<TEntity, TDto , TCreationDto> : IGenericRepository<TEntity, TDto, TCreationDto>
    {
        Task<bool> IsDuplicatedCode(Guid id, string code);

        new Task<PagedResults<TDto>> GetAllAsync();
    }
}

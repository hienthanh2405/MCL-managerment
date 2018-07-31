using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("[controller]")]
    public class CodeGenericController<TEntity, TDto, TCreationDto> : GenericController<TEntity, TDto, TCreationDto> where TDto : class where TCreationDto : class where TEntity : CodeBaseEntity
    {
        private readonly ICodeGenericRepository<TEntity, TDto, TCreationDto> _genericRepository;
        private readonly DatabaseContext _context;
        private readonly DbSet<TEntity> _entities;

        public CodeGenericController(ICodeGenericRepository<TEntity, TDto, TCreationDto> genericRepository, DatabaseContext context ) : base(genericRepository, context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
            _genericRepository = genericRepository;
        }
     
        [HttpGet("duplicated/{id}/{code}")]
        public async Task<IActionResult> IsDuplicatedCodeAsync(Guid id, string code)
        {
            var isDuplicated = await _genericRepository.IsDuplicatedCode( id, code);
            return Ok(new DuplicatedCheckReturnDto(isDuplicated));
        }
    }
}

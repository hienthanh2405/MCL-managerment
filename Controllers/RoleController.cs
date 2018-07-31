using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("Roles")]
    [Authorize]
    public class RoleController : Controller
    {

        private readonly RoleManager<RoleEntity> _roleManager;

        private readonly DatabaseContext _context;
        private readonly DbSet<RoleEntity> _entity;
        private readonly IRoleRepository _roleRepository;

        public RoleController(
            RoleManager<RoleEntity> roleManager,
            DatabaseContext context,
            IRoleRepository roleRepository
            )
        {
            
            _roleManager = roleManager;
            _context = context;
            _roleRepository = roleRepository;
            _entity = _context.Set<RoleEntity>();

        }

        [Route("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var handledData = await _roleRepository.GetAllAsync();

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new { data = items, totalSize });
        }


        public async Task<IActionResult> GetListAsync(
          [FromQuery] int offset,
          [FromQuery] int limit,
          [FromQuery] string keyword,
          [FromQuery] SortOptions<RoleDto, RoleEntity> sortOptions,
          [FromQuery] FilterOptions<RoleDto, RoleEntity> filterOptions)
        {

            IQueryable<RoleEntity> querySearch;

            querySearch = _entity.Where(x =>
                 x.Name.Contains(keyword)
                );

            var handledData = await _roleRepository.GetListAsync(offset, limit, keyword, sortOptions, filterOptions, querySearch);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new { data = items, totalSize });
        }

        [HttpPost]
        public async virtual Task<IActionResult> CreateEntityAsync([FromBody] RoleDto creationDto)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var ProductId = await _roleRepository.CreateAsync(creationDto);
                return Created("", new { id = ProductId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
        }
        [HttpPut("{id}")]
        public async virtual Task<IActionResult> UpdateAsync(Guid id, [FromBody] RoleDto creationDto)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var ProductId = await _roleRepository.EditAsync(id, creationDto);
                return Ok(new { id = ProductId });

            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }

        }

        [HttpDelete("{id}")]
        public async virtual Task<IActionResult> DeleteAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiError(ModelState));
            }
            try
            {
                var roleId = await _roleRepository.DeleteAsync(id);
                return Ok(new { id = roleId });

            }
            catch (Exception ex)
            {
                string name = ex.GetType().Name;
                if (name == "DbUpdateException")
                {
                    string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
                    // convert CamelCase to lower_case
                    string keyLanguage = "can_not_delete_" + Regex.Replace(controllerName, @"(\p{Ll})(\p{Lu})", "$1_$2").ToLower();
                    return BadRequest(new ExceptionResponse("DbUpdateException", keyLanguage));
                }
                return BadRequest(new ExceptionResponse(ex.Message));
            }
        }

    }
}

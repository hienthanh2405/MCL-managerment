using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("Users")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly UserManager<UserEntity> _userManager;

        private readonly DatabaseContext _context;
        private readonly DbSet<UserEntity> _entity;
        private readonly IUserRepository _userRepository;

        public UserController(
            IOptions<IdentityOptions> identityOptions,
            SignInManager<UserEntity> signInManager,
            UserManager<UserEntity> userManager,
            DatabaseContext context,
            IUserRepository userRepository
            )
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _userRepository = userRepository;
            _entity = _context.Set<UserEntity>();

        }

        public async Task<IActionResult> GetUsersAsync(
          [FromQuery] int offset,
          [FromQuery] int limit,
          [FromQuery] string keyword,
          [FromQuery] SortOptions<UserDto, UserEntity> sortOptions,
          [FromQuery] FilterOptions<UserDto, UserEntity> filterOptions)
        {

            IQueryable<UserEntity> querySearch;

            querySearch = _entity.Where(x =>
                 x.FirstName.Contains(keyword)
                 || x.LastName.Contains(keyword)
                 || x.PhoneNumber.Contains(keyword)
                 || x.Email.Contains(keyword)
                );

            var handledData = await _userRepository.GetListAsync(offset, limit, keyword, sortOptions, filterOptions, querySearch);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new { data = items, totalSize });
        }

        [HttpPost]
        public async virtual Task<IActionResult> CreateEntityAsync([FromBody] UserForCreationDto creationDto)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var ProductId = await _userRepository.CreateAsync(creationDto);
                return Created("", new { id = ProductId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async virtual Task<IActionResult> UpdateAsync(Guid id, [FromBody] UserForUpdationDto updationDto)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var userId = await _userRepository.UpdateAsync(id, updationDto);
                return Ok(new { id = userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
        }

        [HttpPut]
        [Route("status/{id}/{status}")]
        public async virtual Task<IActionResult> ChangeStatusAsync( Guid id, bool status)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            try
            {
                var userId = await _userRepository.ChangeStatus(id, status);
                return Ok(new { id = userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
        }
        [HttpPut]
        [Route("password/{id}")]
        public async virtual Task<IActionResult> ResetPassword(Guid id, [FromBody] UserForResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));
            try
            {
                var userId = await _userRepository.ResetPassword(id, resetPasswordDto);
                return Ok(new { id = userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
        }

        [Route("self")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUsersAsync()
        {
            try
            {
                var user = await _userRepository.GetCurrentUser();
    
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }

        }

    }
}

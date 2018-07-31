using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("Customers")]
    [Authorize]
    public class CustomerController : CodeGenericController<CustomerEntity, CustomerDto, CustomerDto>
    {
        private readonly ICodeGenericRepository<CustomerEntity, CustomerDto, CustomerDto> _codeGenericRepository;
        private readonly DatabaseContext _context;
        private readonly DbSet<CustomerEntity> _entity;

        public CustomerController(ICodeGenericRepository<CustomerEntity, CustomerDto, CustomerDto> codeGenericRepository, 
            DatabaseContext context)
            : base(codeGenericRepository, context)
        {
            _codeGenericRepository = codeGenericRepository;
            _context = context;
            _entity = _context.Set<CustomerEntity>();
        }

        public async Task<IActionResult> GetCustomersAsync(
             [FromQuery] int offset,
             [FromQuery] int limit,
             [FromQuery] string keyword,
             [FromQuery] SortOptions<CustomerDto, CustomerEntity> sortOptions,
             [FromQuery] FilterOptions<CustomerDto, CustomerEntity> filterOptions)
        {
            IQueryable<CustomerEntity> querySearch = _entity.Where(x => x.FirstName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
             || x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
             || (x.LastName+ " " + x.FirstName).Contains(keyword, StringComparison.OrdinalIgnoreCase)
             || x.LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase)
             || x.Phone.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Address.Contains(keyword, StringComparison.OrdinalIgnoreCase)
             || x.CompanyName.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.CompanyAddress.Contains(keyword, StringComparison.OrdinalIgnoreCase)
             || x.CompanyTaxCode.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.CompanyPhone.Contains(keyword, StringComparison.OrdinalIgnoreCase)
             || x.CompanyEmail.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.ShipAddress.Contains(keyword, StringComparison.OrdinalIgnoreCase)
             || x.ShipPhone.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.ShipContactName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
             );

            try
            {
                DateTime searchDate = DateTime.ParseExact(keyword, "dd/MM/yyyy",
                                      System.Globalization.CultureInfo.InvariantCulture).Date;
                querySearch = _entity.Where(x => x.BirthDate.Date == searchDate);            
            }
            catch (Exception)
            {
            }

            var handledData = await _codeGenericRepository.GetListAsync(offset, limit, keyword, sortOptions, filterOptions, querySearch);

            var items = handledData.Items.ToArray();
            int totalSize = handledData.TotalSize;

            return Ok(new { data = items, totalSize });
        }


    }
}

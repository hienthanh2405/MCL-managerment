using API.Entities;
using API.Infrastructure;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("dashboard")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly DatabaseContext _context;

        public DashboardController(IDashboardRepository dashboardRepository, DatabaseContext context)
        {
            _dashboardRepository = dashboardRepository;
            _context = context;
        }
        [Route("saleMoney")]
        [HttpPost]
        public async Task<IActionResult> GetDashboardSaleMoneyAsync( [FromBody] DashboardSaleMoneyForRequestDto request)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0);
            request.FromDate = request.FromDate.Date + ts;

            TimeSpan ts2 = new TimeSpan(23, 59, 59);
            request.ToDate = request.ToDate.Date + ts2;

            var result = await _dashboardRepository.GetDashboardSaleMoney(request.FromDate, request.ToDate);

            return Ok(new { data = result });
        }

    }
}

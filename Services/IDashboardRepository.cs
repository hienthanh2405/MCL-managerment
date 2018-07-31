using API.Entities;
using API.Models;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IDashboardRepository
    {
        Task<DashboardSaleMoneyForReturnDto> GetDashboardSaleMoney(DateTime FromDate, DateTime ToDate);
    }
}

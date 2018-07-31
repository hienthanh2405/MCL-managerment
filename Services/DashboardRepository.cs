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
    public class DashboardRepository : IDashboardRepository
    {
        private DatabaseContext _context;
        

        public DashboardRepository(DatabaseContext context)
        {
            _context = context;
          
        }

        public async Task<DashboardSaleMoneyForReturnDto> GetDashboardSaleMoney(DateTime FromDate, DateTime ToDate)
        {
            var bills = await _context.Bills.Where(p => p.CreatedDateTime >= FromDate && p.CreatedDateTime <= ToDate && p.IsActive == true).ToListAsync();
            double totalMoney = 0;
            double totalCash = 0;
            double totalBank = 0;
            double totalCard = 0;
            foreach(BillEntity bill in bills)
            {
                totalMoney += bill.TotalMoney;
                totalCash += bill.PaymentCash;
                totalBank += bill.PaymentBank;
                totalCard += bill.PaymentCard;
            }
            return new DashboardSaleMoneyForReturnDto
            {
                TotalMoney = totalMoney,
                TotalBank = totalBank,
                TotalCard = totalCard,
                TotalCash = totalCash
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Infrastructure;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class InOutReportRepository : IInOutReportRepository
    {
        private DatabaseContext _context;
        private List<ProductStorageEntity> _allProductStorages;
        private List<WarehousingEntity> _allWarehousings;
        private List<BillEntity> _allBills;

        public InOutReportRepository(DatabaseContext context)
        {
            _context = context;
            _allProductStorages = context.ProductStorages.ToList();
            _allWarehousings = context.Warehousings.ToList();
            _allBills = context.Bills.ToList();

        }

        public async Task<PagedResults<InOutReportDto>> GetInOutReportListAsync(int offset, int limit, 
            string keyword, Guid storageId,
            DateTime fromDate, DateTime toDate)
        {
            IQueryable<ProductEntity> productQuery = _context.Products;
            if (keyword != null)
            {
                productQuery = productQuery
                    .Where(p => p.Name.Contains(keyword))
                    .Where(p => p.Code.Contains(keyword));
            };

            productQuery = productQuery.OrderBy(p => p.Code);

            int totalSize = await productQuery.CountAsync();
          
            productQuery = productQuery
            .Skip(offset * limit)
            .Take(limit);

            var products = await productQuery.ToArrayAsync();
            var inOutReportDtos = new List<InOutReportDto>();

            foreach(var product in products)
            {
                var inOutReport = await _convertToInOutReport(product , storageId , fromDate, toDate);
                inOutReportDtos.Add(inOutReport);
            }

            return new PagedResults<InOutReportDto>()
            {
                TotalSize = totalSize,
                Items = inOutReportDtos
            };

        }

        public async Task<PagedResults<InOutReportDto>> GetInOutReportAllAsync(Guid storageId, 
            DateTime fromDate, DateTime toDate)
        {
            IQueryable<ProductEntity> productQuery = _context.Products;
           
            var products = await productQuery.OrderBy(p => p.Code).ToArrayAsync();
            var inOutReportDtos = new List<InOutReportDto>();

            foreach (var product in products)
            {
                var inOutReport = await _convertToInOutReport(product, storageId, fromDate, toDate);
                inOutReportDtos.Add(inOutReport);
            }

            return new PagedResults<InOutReportDto>()
            {
                TotalSize = inOutReportDtos.Count,
                Items = inOutReportDtos
            };

        }

        private  async Task<InOutReportDto> _convertToInOutReport(ProductEntity product, Guid storageId,
            DateTime firstTermDate, DateTime lastTermDate)
        {
            Guid productId = product.Id;

            double inputAmount = _calculateTotalInput(productId , firstTermDate, lastTermDate);
            double outputAmount = _calculateTotalOutput(productId, firstTermDate , lastTermDate);

            double firstTermInventory = await _calculateInventoryFromStartToMilStone(storageId, productId , 
               firstTermDate.AddDays(-1));

            double lastTermInventory = firstTermInventory + inputAmount - outputAmount;

            return new InOutReportDto()
            {
                InputAmount = inputAmount,
                OutputAmount = outputAmount,
                FirstTermInventory = firstTermInventory,
                LastTermInventory = lastTermInventory,
                ProductCode = product.Code,
                ProductName = product.Name
            };
                   
        }


        private async Task<double> _calculateInventoryFromStartToMilStone(Guid storageId, Guid productId, DateTime mileStone)
        {
            await Task.Delay(0);
            double initalInventory = 0;

            //var productStorage = await _context.ProductStorages.SingleOrDefaultAsync(ps =>
            //ps.StorageId == storageId
            //&& ps.ProductId == productId);

            var productStorage = _allProductStorages.SingleOrDefault(ps =>
            ps.StorageId == storageId
            && ps.ProductId == productId);

            if (productStorage == null)
            {
                throw new Exception("Can not find productStorage with storageId=" + storageId + ",productId=" + productId);
            }
            var capitalPriceTrackings = DataHelper<CapitalPriceTrackingDto>.ProtectNullJsonParse(productStorage.CapitalPriceTrackings);
           
                if (capitalPriceTrackings != null && capitalPriceTrackings.Count > 0 && capitalPriceTrackings[0] != null)
                {
                    initalInventory = capitalPriceTrackings[0].Inventory;
                }      
         
            double totalInput = _calculateTotalInput(productId, DateTime.MinValue , mileStone);
            double totalOutput= _calculateTotalOutput(productId, DateTime.MinValue, mileStone);

            return initalInventory + totalInput - totalOutput;
            
        }


        private double _calculateTotalInput(Guid productId, DateTime fromDate , DateTime toDate)
        {
            // get total input amount
            double totalInput = 0;
            //var warehousings = _context.Warehousings.Where(w => w.IsActive == true
            //&& w.ProductList.Contains(productId.ToString())
            //&& w.InputDate.Date <= toDate.Date
            //);

            var warehousings = _allWarehousings.Where(w => w.IsActive == true
           && w.ProductList.Contains(productId.ToString())
           && w.InputDate.Date <= toDate.Date
           );

            // if fromDate is not 01-01-0001, we will apply it 
            if (fromDate != DateTime.MinValue)
            {
                warehousings = warehousings.Where(w => w.InputDate.Date >= fromDate.Date);
            }
            foreach (var warehousing in warehousings)
            {
                var productList = DataHelper<ProductForWareshousingCreationDto>.ProtectNullJsonParse(
                    warehousing.ProductList);
                if (productList == null)
                {
                    continue;
                }
                var matchedProduct = productList.SingleOrDefault(p => p.Id == productId);
                if (matchedProduct == null)
                {
                    continue;
                }
                totalInput += matchedProduct.InputAmount;
            }

            return totalInput;

        }

        private double _calculateTotalOutput(Guid productId, DateTime fromDate, DateTime toDate)
        {
            double totalOutput = 0;

            //var bills = _context.Bills.Where(b => b.IsActive == true
            //   && b.ProductList.Contains(productId.ToString())
            //   && b.CreatedDateTime.Date <= toDate.Date
            //);

            var bills = _allBills.Where(b => b.IsActive == true
               && b.ProductList.Contains(productId.ToString())
               && b.CreatedDateTime.Date <= toDate.Date
            );

            if (fromDate != DateTime.MinValue)
            {
                bills = bills.Where(b => b.CreatedDateTime.Date >= fromDate.Date);
            }

            foreach (var bill in bills)
            {
                var productList = DataHelper<ProductForBillCreationDto>.ProtectNullJsonParse(bill.ProductList);
                if (productList == null)
                {
                    continue;
                }
                var matchedProduct = productList.SingleOrDefault(p => p.Id == productId);
                if (matchedProduct == null)
                {
                    continue;
                }
                totalOutput += matchedProduct.Amount;
            }

            return totalOutput;

        }

    }
}
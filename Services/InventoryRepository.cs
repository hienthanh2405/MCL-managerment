using API.Entities;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Threading.Tasks;
using API.Helpers;
using Newtonsoft.Json;
using API.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace API.Services
{
    public class InventoryRepository : CodeGenericRepository<InventoryEntity, InventoryDto, InventoryForCreationDto>, IInventoryRepository
    {
        private DatabaseContext _context;
        private DbSet<InventoryEntity> _entity;
        private DbSet<CustomerEntity> _customerEntity;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InventoryRepository(DatabaseContext context, 
            UserManager<UserEntity> userManager ,
            IHttpContextAccessor httpContextAccessor
            ) : base(context)
        {
            _context = context;
            _entity = _context.Set<InventoryEntity>();
            _customerEntity = _context.Set<CustomerEntity>();
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResults<InventoryDto>> GetListAsync(int offset, int limit, string keyword, 
            ICollection<Guid> storageIds, SortOptions<InventoryDto, InventoryEntity> sortOptions, 
            FilterOptions<InventoryDto, InventoryEntity> filterOptions)
        {

            IQueryable<InventoryEntity> querySearch;

            querySearch = _entity.Where(x =>
                  x.Code.Contains(keyword)
               || x.Note.Contains(keyword)
               || x.Storage.Name.Contains(keyword)
               || x.ProductList.Contains(keyword)
               );

            try
            {
                DateTime searchDate = DateTime.ParseExact(keyword, "dd/MM/yyyy",
                                         System.Globalization.CultureInfo.InvariantCulture).Date;
                querySearch = _entity.Where(x => x.CreatedDateTime.Date == searchDate.Date || x.BalanceDateTime == searchDate.Date);

            }
            catch (Exception)
            {
            }

            IQueryable<InventoryEntity> query = _entity;
            query = sortOptions.Apply(query);
            query = filterOptions.Apply(query);
            if (keyword != null)
            {
                query = querySearch;
            }

            query = query.Where(w => storageIds.Contains(w.StorageId));

            var items = await query
                .Skip(offset * limit)
                .Take(limit)
                .ProjectTo<InventoryDto>()
                .ToArrayAsync();

            var size = await query.CountAsync();

            return new PagedResults<InventoryDto>
            {
                Items = items,
                TotalSize = size
            };

        }

        new public async Task<Guid> CreateAsync(InventoryForCreationDto inventoryForCreationDto)
        {
            
            var newInventory = new InventoryEntity();

            foreach (PropertyInfo propertyInfo in inventoryForCreationDto.GetType().GetProperties())
            {
                if (newInventory.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    newInventory.GetType().GetProperty(propertyInfo.Name).SetValue(newInventory, propertyInfo.GetValue(inventoryForCreationDto, null));
                }
            }
            newInventory.CreatedDateTime = DateTime.Now;
            newInventory.Status = CONSTANT.INVENTORY_INVENTORIED;

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            newInventory.CreatedUserId = user.Id;

            await _entity.AddAsync(newInventory);

            var created = await _context.SaveChangesAsync();
            if (created < 1)
            {
                throw new InvalidOperationException("Database context could not create data.");
            }
            return newInventory.Id;
        }
    
        new public async Task<Guid> EditAsync(Guid id, InventoryForCreationDto billForCreationDto)
        {
            await Task.Run(() => { });
            throw new Exception("Inventory does not allow for editing");
        }
        public async Task<Guid> DestroyAsync(Guid id)
        {
            var inventory = await _entity.SingleOrDefaultAsync(r => r.Id == id);
            if (inventory == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }
            if (inventory.Status == CONSTANT.INVENTORY_BANLANCED)
            {
                throw new InvalidOperationException("Inventory has balanced status does not allow for destroying");
            }
            inventory.Status = CONSTANT.INVENTORY_DESTROY;

            _entity.Update(inventory);

            await _context.SaveChangesAsync();
            return inventory.Id;
        }
        public async Task<Guid> BalanceAsync(Guid id)
        {
            var inventory = await _entity.SingleOrDefaultAsync(r => r.Id == id);
            if (inventory == null)
            {
                throw new Exception("Can not find object with this Id.");
            }
            if(inventory.Status == CONSTANT.INVENTORY_DESTROY || inventory.Status == CONSTANT.INVENTORY_BANLANCED)
            {
                throw new Exception("Inventory has balanced or destroy status does not allow for balancing");
            }
            inventory.BalanceDateTime = DateTime.Now;

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            inventory.BalanceUserId = user.Id;

            ProductForBalanceInventoryDto[] products = JsonConvert.DeserializeObject<ProductForBalanceInventoryDto[]>(inventory.ProductList);

            foreach (var product in products)
            {
                var productStorage = await _context.ProductStorages.SingleOrDefaultAsync(p => p.StorageId == inventory.StorageId && p.ProductId == product.Id);
                if(productStorage == null)
                {
                    throw new Exception("Can not find productStorage with productId");
                }
                productStorage.Inventory = product.RealInventory;

                // we need to update CapitalPriceTrackings
                productStorage.CapitalPriceTrackings = productStorage.CapitalPriceTrackings ?? "[]";

                var capitalPriceTrackings =  JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>( productStorage.CapitalPriceTrackings);
                var lastestCapitalPriceTracking = capitalPriceTrackings.Last();
                var newCapitalPriceTracking = new CapitalPriceTrackingDto()
                {
                    InventoryId = id,
                    Inventory = product.RealInventory,
                    Amount = 0,
                    CapitalPrice = lastestCapitalPriceTracking.CapitalPrice,
                    InputPrice = 0
                };
                capitalPriceTrackings.Add(newCapitalPriceTracking);
                productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);

                _context.ProductStorages.Update(productStorage);

            }

            inventory.Status = CONSTANT.INVENTORY_BANLANCED;
            _entity.Update(inventory);

           var updated = await _context.SaveChangesAsync();

            if (updated < 1)
            {
                throw new Exception("Context can not updated");
            }

            return inventory.Id;
        }

        public async Task<InventoryReportDto> GetReport(DateTime fromDate, DateTime toDate, ICollection<Guid> storageIds)
        {
            var inventoryReport = new InventoryReportDto
            {
                ReturnStorageInventoryList = new List<ReturnStorageInventoryDto>(),
                StorageNameList = new List<string>(),
                FromDate = fromDate,
                ToDate = toDate
            };

            foreach (var storageId in storageIds)
            {
                var storage = await _context.Storages.SingleAsync(s => s.Id == storageId);
                inventoryReport.StorageNameList.Add(storage.Name);

                var inventoryList = await _entity.Where(b => b.StorageId == storageId
                && b.CreatedDateTime.Date >= fromDate.Date
                && b.CreatedDateTime.Date <= toDate.Date
                && b.Status != CONSTANT.INVENTORY_DESTROY
                ).Include(i => i.Storage).Include(i => i.BalanceUser).ToListAsync();

                inventoryReport.InventoryAmount += inventoryList.Count;
             
                foreach (var inventory in inventoryList)
                {
                    inventoryReport.TotalDeviation += inventory.TotalDeviation;
                    inventoryReport.IncreaseDeviation += inventory.IncreaseDeviation;
                    inventoryReport.DecreaseDeviation += inventory.DecreaseDeviation;

                    if (inventory.Status == CONSTANT.INVENTORY_BANLANCED)
                    {
                        inventoryReport.BalanceInventoryAmount++;

                    } else if(inventory.Status == CONSTANT.INVENTORY_INVENTORIED)
                    {
                        inventoryReport.InventoriedInventoryAmount++;
                    }
                }
                ReturnStorageInventoryDto storageWarehousingDto = new ReturnStorageInventoryDto
                {
                    StorageName = storage.Name,
                    InventoryList = Mapper.Map<List<InventoryDto>>(inventoryList)
                };
                inventoryReport.ReturnStorageInventoryList.Add(storageWarehousingDto);
            }
            return inventoryReport;
        }
    }
}

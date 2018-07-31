using API.Entities;
using API.Helpers;
using API.Infrastructure;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Services
{
    public class ProductStorageRepository : GenericRepository<ProductStorageEntity, ProductStorageDto, ProductStorageForCreationDto>, IProductStorageRepository
    {
        private DatabaseContext _context;
        private DbSet<ProductStorageEntity> _entity;

        public ProductStorageRepository(DatabaseContext context) : base(context)
        {
            _context = context;
            _entity = _context.Set<ProductStorageEntity>();
        }

        public async Task<PagedResults<ProductStorageDto>> GetListAsync(int offset, int limit, string keyword,
        SortOptions<ProductStorageDto, ProductStorageEntity> sortOptions,
         FilterOptions<ProductStorageDto, ProductStorageEntity> filterOptions,
         Guid storageId,
         string inventoryStatus = "ALL"
         )
        {

            IQueryable<ProductStorageEntity> query = _entity.Include(p => p.Product).Include(p => p.Storage).OrderBy(p => p.Product.Code);

            query = sortOptions.Apply(query);
            query = filterOptions.Apply(query);
         
            if (keyword != null)
            {
                query = query.Where(
                x => x.Product.Name.Contains(keyword)
                || x.Product.Code.Contains(keyword)
                );
            }

            if (inventoryStatus == CONSTANT.PRODUCT_STORAGE_HAS_INVENTORY)
            {
                query = query.Where(x => x.Inventory > 0);
             
            }
            else if (inventoryStatus == CONSTANT.PRODUCT_STORAGE_SOLD_OUT)
            {
                query = query.Where(x => x.Inventory <= 0);
            }

            if (storageId != Guid.Empty)
            {
                query = query.Where(x => x.StorageId == storageId);
            }

            var size = await query.CountAsync();

            var productStorageEntities = await query
                .Skip(offset * limit)
                .Take(limit)
                .ToListAsync();

            var productStorageDtos = Mapper.Map<List<ProductStorageDto>>(productStorageEntities);
            for(int i = 0; i < productStorageDtos.Count ; i++)
            {
                productStorageDtos[i].ProductProductionDateList = (productStorageEntities[i].ProductProductionDateList != null) ?
                    JsonConvert.DeserializeObject<List<ProductProductionDateDto>>(productStorageEntities[i].ProductProductionDateList)
                    : new List<ProductProductionDateDto>();

                productStorageDtos[i].Product.UnitConverterList = (productStorageEntities[i].Product.UnitConverterList != null) ?
                    JsonConvert.DeserializeObject<List<UnitConverterDto>>(productStorageEntities[i].Product.UnitConverterList)
                    : new List<UnitConverterDto>();

                productStorageDtos[i].Product.Inventory = productStorageDtos[i].Inventory;
                productStorageDtos[i].Product.CapitalPrice = productStorageDtos[i].CapitalPrice;

                productStorageDtos[i].ProductProductionDateList = productStorageDtos[i].ProductProductionDateList.OrderBy(p => p.ProductionDate).ToList();
            }

            return new PagedResults<ProductStorageDto>
            {
                TotalSize = size,
                Items = productStorageDtos
            };

        }

        new public async Task<Guid> CreateAsync(ProductStorageForCreationDto productStorageForCreationDto)
        {

            var newEntity = new ProductStorageEntity();

            var existedProductStorage = _entity.FirstOrDefault(p => p.Product.Id == productStorageForCreationDto.ProductId
           && p.Storage.Id == productStorageForCreationDto.StorageId);
            if (existedProductStorage != null)
            {
                throw new InvalidOperationException("Product storage is existed on database");
            }

            foreach (PropertyInfo propertyInfo in productStorageForCreationDto.GetType().GetProperties())
            {
                if (newEntity.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    newEntity.GetType().GetProperty(propertyInfo.Name).SetValue(newEntity, propertyInfo.GetValue(productStorageForCreationDto, null));
                }
            }

            await _entity.AddAsync(newEntity);

            var created = await _context.SaveChangesAsync();
            if (created < 1)
            {
                throw new InvalidOperationException("Database context could not create data.");
            }
            return newEntity.Id;
        }

        new public async Task<Guid> EditAsync(Guid id, ProductStorageForCreationDto productStorageForCreationDto)
        {
            var entity = await _entity.SingleOrDefaultAsync(r => r.Id == id);
            if (entity == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }

            var existedProductStorage = _entity.FirstOrDefault(p => p.Product.Id == productStorageForCreationDto.ProductId
          && p.Storage.Id == productStorageForCreationDto.StorageId && p.Id != id);
            if (existedProductStorage != null)
            {
                throw new InvalidOperationException("Product storage is existed on database");
            }

            foreach (PropertyInfo propertyInfo in productStorageForCreationDto.GetType().GetProperties())
            {
                string key = propertyInfo.Name;
                if (key != "Id" && entity.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    entity.GetType().GetProperty(key).SetValue(entity, propertyInfo.GetValue(productStorageForCreationDto, null));
                }
            }

            _entity.Update(entity);
            var updated = await _context.SaveChangesAsync();
            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }
            return id;


        }

        public async Task<ProductStorageDto> GetByStorageIdAndProductIdAsync(Guid storageId , Guid productId)
        {
            var entity = await _entity.SingleOrDefaultAsync(r => r.StorageId == storageId && r.ProductId == productId);
            if (entity == null)
            {
                throw new InvalidOperationException("Can not find object with this storageId and productId.");
            }
            return Mapper.Map<ProductStorageDto>(entity);
        }

        public async Task<ProductStorageReportDto> GetReport(string inventoryStatus, ICollection<Guid> storageIds)
        {
            var productStorageReport = new ProductStorageReportDto
            {
                ReturnProductStorageList = new List<ReturnProductStorageDto>(),
                StorageNames = new List<string>(),
                CreatedDateTime = DateTime.Now
            };

            foreach (var storageId in storageIds)
            {
                var storage = await _context.Storages.SingleAsync(s => s.Id == storageId);
                productStorageReport.StorageNames.Add(storage.Name);

                IQueryable<ProductStorageEntity> querySearch = _entity.Where( p => p.StorageId == storage.Id).OrderBy(p => p.Product.Code);

                if (inventoryStatus == CONSTANT.PRODUCT_STORAGE_HAS_INVENTORY)
                {
                    querySearch = querySearch.Where(x => x.Inventory > 0);
                }
                else if (inventoryStatus == CONSTANT.PRODUCT_STORAGE_SOLD_OUT)
                {
                    querySearch = querySearch.Where(x => x.Inventory <= 0);
                }

                var productStorageEntities = await querySearch.Include(p =>p.Product).Include(p => p.Storage).ToListAsync();

                foreach (var productStorage in productStorageEntities)
                {
                    productStorageReport.Inventory += productStorage.Inventory;
                }
                // convert ProductProductionDateList 
                List<ProductStorageDto> productStorageDtos = Mapper.Map<List<ProductStorageDto>>(productStorageEntities);
                for(int i = 0; i < productStorageEntities.Count; i++)
                {
                    productStorageDtos[i].ProductProductionDateList = (productStorageEntities[i].ProductProductionDateList == null) ?
                        new List<ProductProductionDateDto>() : JsonConvert.DeserializeObject<List<ProductProductionDateDto>>(productStorageEntities[i].ProductProductionDateList);
                }

                ReturnProductStorageDto returnProductStorageDto = new ReturnProductStorageDto
                {
                    StorageName = storage.Name,
                    ProductStorageList = productStorageDtos
                };
                productStorageReport.ReturnProductStorageList.Add(returnProductStorageDto);
            }
            return productStorageReport;
        }

        public async Task<List<CapitalPriceTrackingDetailDto>> GetCapitalPriceHistory(Guid productStorageId)
        {

            var productStorage = await _entity.SingleOrDefaultAsync(ps => ps.Id == productStorageId);
            if(productStorage == null)
            {
                throw new Exception("Can not find product storage with id=" + productStorageId);
            }
            var capitalPriceTrackings = (productStorage.CapitalPriceTrackings == null) ?
                new List<CapitalPriceTrackingDto>() : JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);

            List<CapitalPriceTrackingDetailDto> capitalPriceTrackingDetails = new List<CapitalPriceTrackingDetailDto>();
            
            for(int i = 0; i < capitalPriceTrackings.Count; i++)
            {
                CapitalPriceTrackingDetailDto capitalPriceTrackingDetail = Mapper.Map<CapitalPriceTrackingDetailDto>( capitalPriceTrackings[i]);
                capitalPriceTrackingDetail.Type = _getTrackingType(capitalPriceTrackings[i]);
                capitalPriceTrackingDetail.Code = await _getCode(capitalPriceTrackings[i]);
                capitalPriceTrackingDetails.Add(capitalPriceTrackingDetail);
            }
            return capitalPriceTrackingDetails;
        }

        private string _getTrackingType( CapitalPriceTrackingDto capitalPriceTracking)
        {
           if(capitalPriceTracking.BillId == Guid.Empty && capitalPriceTracking.WarehousingId == Guid.Empty && capitalPriceTracking.InventoryId == Guid.Empty)
            {
                return "inital_creating";
            } else if( capitalPriceTracking.BillId != Guid.Empty)
            {
                return "bill";
            } else if( capitalPriceTracking.WarehousingId != Guid.Empty)
            {
                return "warehousing";
            }else if(capitalPriceTracking.InventoryId != Guid.Empty)
            {
                return "inventory";
            }
            return "";
        }

        private async Task<string> _getCode(CapitalPriceTrackingDto capitalPriceTracking)
        {
            if (capitalPriceTracking.BillId != Guid.Empty)
            {
                var bill = await _context.Bills.SingleOrDefaultAsync(b => b.Id == capitalPriceTracking.BillId);
                return bill.Code;
            }
            if(capitalPriceTracking.WarehousingId != Guid.Empty)
            {
                var warehousing = await _context.Warehousings.SingleOrDefaultAsync(w => w.Id == capitalPriceTracking.WarehousingId);
                return warehousing.Code;
            }
            if(capitalPriceTracking.InventoryId != Guid.Empty)
            {
                var inventory = await _context.Inventories.SingleOrDefaultAsync(i => i.Id == capitalPriceTracking.InventoryId);
                return inventory.Code;
            }

            return "";
        }


        public async Task<bool> FixAllCapitalPriceTracking()
        {
            bool finished = false; 

            var bills = await _context.Bills.Where(b => b.IsActive == true).ToListAsync();
            foreach(var bill in bills)
            {
               var productList = JsonConvert.DeserializeObject<List<ProductForBillCreationDto>>(bill.ProductList);
               foreach(var product in productList)
                {
                    var productStorage = await _context.ProductStorages.SingleOrDefaultAsync( ps=> ps.ProductId == product.Id && ps.StorageId == bill.StorageId);
                    var capitalPriceTrackings = (productStorage.CapitalPriceTrackings == null) ?
                        new List<CapitalPriceTrackingDto>() : JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);

                    CapitalPriceTrackingDto defaultRecord = new CapitalPriceTrackingDto();

                    if (capitalPriceTrackings.Count == 0)
                    {    
                        capitalPriceTrackings.Add(defaultRecord);                  
                    } else if(capitalPriceTrackings[0].WarehousingId != Guid.Empty && capitalPriceTrackings[0].BillId != Guid.Empty)
                    {
                        capitalPriceTrackings.Add(defaultRecord);
                    }
                  
                    var existed = capitalPriceTrackings.Find(c => c.BillId == bill.Id);
                    if(existed == null)
                    {
                        capitalPriceTrackings.Add(new CapitalPriceTrackingDto {
                            BillId = bill.Id,
                            InputPrice = 0,
                            Amount = product.Amount,
                            CapitalPrice = capitalPriceTrackings[capitalPriceTrackings.Count - 1].CapitalPrice,
                            Inventory = capitalPriceTrackings[capitalPriceTrackings.Count - 1].Inventory + product.Amount
                        });           
                    }
                    productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);
                    _context.ProductStorages.Update(productStorage);
                }
            };
            await _context.SaveChangesAsync();

            var warehousings = await _context.Warehousings.Where(b => b.IsActive == true).ToListAsync();
            foreach (var warehousing in warehousings)
            {
                var productList = JsonConvert.DeserializeObject<List<ProductForWareshousingCreationDto>>(warehousing.ProductList);
                foreach (var product in productList)
                {
                    var productStorage = await _context.ProductStorages.SingleOrDefaultAsync(ps => ps.ProductId == product.Id && ps.StorageId == warehousing.StorageId);
                    var capitalPriceTrackings = (productStorage.CapitalPriceTrackings == null) ?
                        new List<CapitalPriceTrackingDto>() : JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);

                    CapitalPriceTrackingDto defaultRecord = new CapitalPriceTrackingDto();

                    if (capitalPriceTrackings.Count == 0)
                    {
                        capitalPriceTrackings.Add(defaultRecord);
                    }
                    else if (capitalPriceTrackings[0].WarehousingId != Guid.Empty && capitalPriceTrackings[0].BillId != Guid.Empty)
                    {
                        capitalPriceTrackings.Add(defaultRecord);
                    }

                    var existed = capitalPriceTrackings.Find(c => c.WarehousingId == warehousing.Id);
                    if (existed == null)
                    {
                        var lastestRecord = capitalPriceTrackings[capitalPriceTrackings.Count - 1];
                        capitalPriceTrackings.Add(new CapitalPriceTrackingDto
                        {
                            BillId = warehousing.Id,
                            InputPrice = product.InputPrice,
                            Amount = product.InputAmount,
                            CapitalPrice = ProductStorageHelper.CalculateCapitalPrice(lastestRecord.Inventory, lastestRecord.CapitalPrice, product.InputAmount, product.InputPrice),
                            Inventory = lastestRecord.Inventory + product.InputAmount
                        });

                    }
                productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);
                _context.ProductStorages.Update(productStorage);
                }
            };
            await _context.SaveChangesAsync();

           var productStorages = await _context.ProductStorages.ToListAsync();
           foreach(var productStorage in productStorages)
            {
                var capitalPriceTrackings = (productStorage.CapitalPriceTrackings == null) ?
                  new List<CapitalPriceTrackingDto>() : JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);

                CapitalPriceTrackingDto defaultRecord = new CapitalPriceTrackingDto();
                
                if (capitalPriceTrackings.Count == 0 || ! _isDefaultRecord(capitalPriceTrackings[0])  )
                {
                    capitalPriceTrackings.Insert(0,defaultRecord);
                }



                List<int> removeIndexs =  new List<int>();

                for (int i = 1; i < capitalPriceTrackings.Count; i++)
                {
                   
                    if (_isDefaultRecord(capitalPriceTrackings[i]))
                    {
                        removeIndexs.Add(i);
                    }
                    else if (capitalPriceTrackings[i].BillId != Guid.Empty)
                    {
                        capitalPriceTrackings[i].InputPrice = 0;
                        capitalPriceTrackings[i].CapitalPrice = capitalPriceTrackings[i - 1].CapitalPrice;
                        capitalPriceTrackings[i].Inventory = capitalPriceTrackings[i - 1].Inventory - capitalPriceTrackings[i].Amount;

                    } else if(capitalPriceTrackings[i].WarehousingId != Guid.Empty)
                    {
                        capitalPriceTrackings[i].CapitalPrice = ProductStorageHelper
                            .CalculateCapitalPrice(
                            capitalPriceTrackings[i - 1].Inventory, 
                            capitalPriceTrackings[i - 1].CapitalPrice,
                            capitalPriceTrackings[i].Amount, 
                            capitalPriceTrackings[i].InputPrice);

                        capitalPriceTrackings[i].Inventory = capitalPriceTrackings[i - 1].Inventory
                            + capitalPriceTrackings[i].Amount;
                    }
                }
                foreach(int index in removeIndexs)
                {
                    capitalPriceTrackings.RemoveAt(index);
                }
                productStorage.Inventory = capitalPriceTrackings[capitalPriceTrackings.Count - 1].Inventory;
                productStorage.CapitalPrice = capitalPriceTrackings[capitalPriceTrackings.Count - 1].CapitalPrice;
                productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);
                _context.ProductStorages.Update(productStorage);
            }
            await _context.SaveChangesAsync();

            finished = true;

            return finished;

        }

        private  bool _isDefaultRecord(CapitalPriceTrackingDto capitalPriceTrackingDto)
        {
            return (capitalPriceTrackingDto.BillId == Guid.Empty && capitalPriceTrackingDto.WarehousingId == Guid.Empty && capitalPriceTrackingDto.InventoryId == Guid.Empty);
        }

        public async Task<List<CapitalPriceTrackingDetailDto>> ChangeInitialCapitalPriceTracking(Guid productStorageId , CapitalPriceTrackingDto updatedDto)
        {
            var productStorage = await _entity.SingleOrDefaultAsync(ps => ps.Id == productStorageId);
            if(productStorage == null)
            {
                throw new Exception("Can not find product storage with id=" + productStorageId);
            }
            var capitalPriceTrackings = (productStorage.CapitalPriceTrackings == null) ?
                 new List<CapitalPriceTrackingDto>() : JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);
            if(capitalPriceTrackings.Count > 0)
            {
                if(_isDefaultRecord (capitalPriceTrackings[0]))
                {
                    capitalPriceTrackings = ProductStorageHelper.UpdateCapitalPriceTracking(0, updatedDto, capitalPriceTrackings);
                    productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);
                    productStorage.Inventory = capitalPriceTrackings[capitalPriceTrackings.Count - 1].Inventory;
                    productStorage.CapitalPrice = capitalPriceTrackings[capitalPriceTrackings.Count - 1].CapitalPrice;
                    _entity.Update(productStorage);
                    await _context.SaveChangesAsync();
                    List<CapitalPriceTrackingDetailDto> capitalPriceTrackingDetails = await GetCapitalPriceHistory(productStorageId);
                    return capitalPriceTrackingDetails;
                } else
                {
                    throw new Exception("This is not default record");
                }
            }
            return null;
        }
        

    }
}

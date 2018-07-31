using API.Entities;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using API.Infrastructure;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using API.Helpers;

namespace API.Services
{
    public class WarehousingRepository : GenericRepository<WarehousingEntity, WarehousingDto, WarehousingForCreationDto>, IWarehousingRepository
    {
        private DatabaseContext _context;
        private DbSet<WarehousingEntity> _entity;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private SettingDto setting;

        public WarehousingRepository(DatabaseContext context, UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            _entity = _context.Set<WarehousingEntity>();
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;

            var settings = _context.Settings.ToArray();
            setting = Mapper.Map<SettingDto>(settings[0]);
        }

        public async Task<PagedResults<WarehousingDto>> GetListAsync(int offset, int limit, string keyword, ICollection<Guid> storageIds, SortOptions<WarehousingDto, WarehousingEntity> sortOptions,
            FilterOptions<WarehousingDto, WarehousingEntity> filterOptions,
             DateTime fromDate, DateTime toDate
            )
        {

            IQueryable<WarehousingEntity> query = _entity;
            query = sortOptions.Apply(query);
            query = filterOptions.Apply(query);
            query = query.Where(w => storageIds.Contains(w.StorageId));

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                query = query.Where(w => w.CreatedDateTime.Date >= fromDate.Date
                && w.CreatedDateTime.Date <= toDate.Date);

            }
            else if (fromDate != DateTime.MinValue)
            {
                query = query.Where(w => w.CreatedDateTime.Date == fromDate.Date);
            }


            if (keyword != null)
            {
                try
                {
                    DateTime searchDate = DateTime.ParseExact(keyword, "dd/MM/yyyy",
                                             System.Globalization.CultureInfo.InvariantCulture).Date;

                    query = query.Where(w =>
                    w.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.Storage.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.Supplier.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.ProductList.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.InputDate.Date == searchDate.Date
                );

                }
                catch (Exception)
                {
                    query = query.Where(w =>
                    w.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.Supplier.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.ProductList.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                   || w.SupplierBillList.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                }
            }

            var items = await query
                .Skip(offset * limit)
                .Take(limit)
                .Include(w => w.Storage)
                .Include(w => w.CreatedUser)
                .Include(w => w.UpdatedUser)
                .Include(w => w.Supplier)
                .ToArrayAsync();

            var totalSize = await query.CountAsync();

            var returnItems = Mapper.Map<List<WarehousingDto>>(items);

            for (var i = 0; i < items.Length; i++)
            {
                returnItems[i].SupllierBillList = JsonConvert.DeserializeObject<List<SupplierBillDto>>(items[i].SupplierBillList);
                returnItems[i].ProductList = JsonConvert.DeserializeObject<List<ProductForWareshousingCreationDto>>(items[i].ProductList);
            }
            return new PagedResults<WarehousingDto>
            {
                Items = returnItems,
                TotalSize = totalSize
            };

        }


        public new async Task<WarehousingDto> GetSingleAsync(Guid id)
        {
            var warehousingEntity = await _entity.Include(w => w.CreatedUser)
                .Include(w => w.UpdatedUser)
                .Include(w => w.Storage)
                .Include(w => w.Supplier)
                .SingleOrDefaultAsync(r => r.Id == id);
            if (warehousingEntity == null)
            {
                throw new InvalidOperationException("Can not find warehousing with id = " + id);
            }
            var warehousingDto = Mapper.Map<WarehousingDto>(warehousingEntity);
            warehousingDto.SupllierBillList = JsonConvert.DeserializeObject<List<SupplierBillDto>>(warehousingEntity.SupplierBillList);
            warehousingDto.ProductList = JsonConvert.DeserializeObject<List<ProductForWareshousingCreationDto>>(warehousingEntity.ProductList);

            return warehousingDto;
        }

        new public async Task<Guid> CreateAsync(WarehousingForCreationDto creationDto)
        {

            var newWarehousing = new WarehousingEntity();
            foreach (PropertyInfo propertyInfo in creationDto.GetType().GetProperties())
            {
                if (newWarehousing.GetType().GetProperty(propertyInfo.Name) != null && propertyInfo.Name != "ProductList" && propertyInfo.Name != "SupplierBillList")
                {
                    newWarehousing.GetType().GetProperty(propertyInfo.Name).SetValue(newWarehousing, propertyInfo.GetValue(creationDto, null));
                }
            }
            newWarehousing.IsActive = true;
            newWarehousing.SupplierBillList = JsonConvert.SerializeObject(creationDto.SupplierBillList);
            newWarehousing.CreatedDateTime = DateTime.Now;

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            newWarehousing.CreatedUserId = user.Id;
            newWarehousing.ProductList = JsonConvert.SerializeObject(creationDto.ProductList);
            newWarehousing.SupplierBillList = JsonConvert.SerializeObject(creationDto.SupplierBillList);

            double productMoney = 0;
            foreach (var product in creationDto.ProductList)
            {
                // calculate product money 
                productMoney += product.InputAmount * product.InputPrice;
            }

            newWarehousing.ProductMoney = productMoney;
            newWarehousing.SummaryMoney = newWarehousing.ProductMoney + newWarehousing.TaxMoney;
            newWarehousing.DebtMoney = newWarehousing.SummaryMoney - creationDto.PaymentMoney;

            // generate the code 
            bool isDuplicated = false;
            do
            {
                string lastestCode = _entity.Max(w => w.Code);
                newWarehousing.Code = CodeGeneratorHelper.GetGeneratedCode(CONSTANT.WAREHOUSING_PREFIX, lastestCode, CONSTANT.GENERATED_NUMBER_LENGTH);
                isDuplicated = await IsDuplicatedCode(newWarehousing.Code);

            } while (isDuplicated);

            await _entity.AddAsync(newWarehousing);

            // update inventory of ProductStorages 

            foreach (var product in creationDto.ProductList)
            {
                var productStorage = await _context.ProductStorages.SingleOrDefaultAsync(p => p.ProductId == product.Id && p.StorageId == newWarehousing.StorageId);
                _addCapitalTracking(product, newWarehousing.StorageId, newWarehousing.Id, productStorage);
                _updateInventoryDetail(product, null, productStorage);
            }

            var created = await _context.SaveChangesAsync();
            if (created < 1)
            {
                throw new InvalidOperationException("Database context could not create Warehousing.");
            }

            return newWarehousing.Id;
        }

        private void _addCapitalTracking(ProductForWareshousingCreationDto product, Guid storageId, Guid warehousingId, ProductStorageEntity productStorage)
        {
            // create capital price tracking 
            productStorage.CapitalPrice = ProductStorageHelper.CalculateCapitalPrice(
             productStorage.Inventory, productStorage.CapitalPrice, product.InputAmount, product.InputPrice
           );

            productStorage.Inventory += product.InputAmount;

            CapitalPriceTrackingDto capitalPriceTracking = new CapitalPriceTrackingDto
            {
                WarehousingId = warehousingId,
                Amount = product.InputAmount,
                InputPrice = product.InputPrice,
                CapitalPrice = productStorage.CapitalPrice,
                Inventory = productStorage.Inventory
            };

            productStorage.CapitalPriceTrackings = productStorage.CapitalPriceTrackings ?? "[]";

            var capitalPriceTrackings = JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);
            capitalPriceTrackings.Add(capitalPriceTracking);
            productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);
            _context.ProductStorages.Update(productStorage);
        }

        private void _updateInventoryDetail(ProductForWareshousingCreationDto editedProduct, ProductForWareshousingCreationDto oldProduct, ProductStorageEntity productStorage)
        {

            // update detail inventory for each product
            var oldProductProductionList = (productStorage.ProductProductionDateList == null) ? new List<ProductProductionDateDto>() : JsonConvert.DeserializeObject<List<ProductProductionDateDto>>(productStorage.ProductProductionDateList);
            var detailInputAmountList = editedProduct.DetailInputAmountList;
            if (detailInputAmountList != null && detailInputAmountList.Count > 0)
            {

                foreach (var detailInputAmount in detailInputAmountList)
                {
                    if (detailInputAmount.InputAmount > editedProduct.InputAmount)
                    {
                        throw new Exception("Detail InputAmount is greater than product InputAmount");
                    }

                    var oldProductProduction = oldProductProductionList.SingleOrDefault(p => p.ProductionWeekYear == detailInputAmount.ProductionWeekYear);
                    if (oldProductProduction != null)
                    {
                        if (oldProduct == null)
                        {
                            oldProductProduction.Inventory += (detailInputAmount.InputAmount);
                        }
                        else
                        {
                            var beforeAddedProductDetailInputAmount = oldProduct.DetailInputAmountList.SingleOrDefault(p => p.ProductionWeekYear == detailInputAmount.ProductionWeekYear);
                            double beforeInputAmount = (beforeAddedProductDetailInputAmount == null) ? 0 : beforeAddedProductDetailInputAmount.InputAmount;
                            oldProductProduction.Inventory += (detailInputAmount.InputAmount - beforeInputAmount);
                        }

                    }
                    else
                    {
                        oldProductProductionList.Add(new ProductProductionDateDto()
                        {
                            ProductionWeekYear = detailInputAmount.ProductionWeekYear,
                            ProductionDate = DateTimeHelper.ConvertWeekYearToDateTime(detailInputAmount.ProductionWeekYear),
                            Inventory = detailInputAmount.InputAmount
                        });
                    }
                }
                productStorage.ProductProductionDateList = JsonConvert.SerializeObject(oldProductProductionList);
                //_context.ProductStorages.Update(productStorage);
            }
        }


        new public async Task<Guid> EditAsync(Guid id, WarehousingForCreationDto updationDto)
        {

            var warehousing = await _entity.SingleOrDefaultAsync(w => w.Id == id);
            if (warehousing == null)
            {
                throw new Exception("Can not find warehousing with id = " + id);
            }
            if (!warehousing.IsActive)
            {
                throw new Exception("The destroyed warehousing can not be edited");
            }

            var oldProducts = JsonConvert.DeserializeObject<List<ProductForWareshousingCreationDto>>(warehousing.ProductList);
            var editedProducts = updationDto.ProductList;

            double productMoney = 0;

            // when a old product is removed in new product list, we remove inventory
            foreach (var oldProduct in oldProducts)
            {
                var editedProduct = editedProducts.SingleOrDefault(p => p.Id == oldProduct.Id);
                if (editedProduct == null)
                {
                    await _removeCapitalTracking(oldProduct, updationDto.StorageId, id);
                }
            }

            foreach (var editedProduct in editedProducts)
            {
                // calculate product money 
                productMoney += editedProduct.InputAmount * editedProduct.InputPrice;
                /* when not finding edited product on old product list, let add a new cappital tracking into 
                 ProductStorage of edited product */

                var oldProduct = oldProducts.SingleOrDefault(p => p.Id == editedProduct.Id);
                if (oldProduct == null)
                {
                    var productStorage = await _context.ProductStorages.SingleOrDefaultAsync(p => p.ProductId == editedProduct.Id
                     && p.StorageId == warehousing.StorageId);
                    _addCapitalTracking(editedProduct, updationDto.StorageId, id, productStorage);
                }
                else
                {
                    var productStorage = await _context.ProductStorages.SingleOrDefaultAsync(p => p.ProductId == editedProduct.Id
                      && p.StorageId == warehousing.StorageId);

                    // if we find a old product is same with edited product, we need to to update ProductProductionDate
                    if (editedProduct.DetailInputAmountList != null && editedProduct.DetailInputAmountList.Count > 0)
                    {
                        _updateInventoryDetail(editedProduct, oldProduct, productStorage);
                    }

                    // if we find old product is same with edited product, we need to update capital tracking of product storage


                    var capitalPriceTrackings = JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);
                    int index = capitalPriceTrackings.FindIndex(c => c.WarehousingId == id);
                    if (index == -1) // hot fix, this shit. duplicate warehousing when create cause error on destroy, it remove all warehousing
                    {
                        var lastestRecord = capitalPriceTrackings[capitalPriceTrackings.Count - 1];

                        CapitalPriceTrackingDto capitalPriceTrackingDto = new CapitalPriceTrackingDto()
                        {
                            WarehousingId = id,
                            BillId = Guid.Empty,
                            InventoryId = Guid.Empty,
                            Amount = editedProduct.InputAmount,
                            Inventory = lastestRecord.Inventory + editedProduct.InputAmount,
                            CapitalPrice = ProductStorageHelper.CalculateCapitalPrice(
                                lastestRecord.Inventory,
                                lastestRecord.CapitalPrice,
                                editedProduct.InputAmount,
                                editedProduct.InputPrice
                            )

                        };
                        capitalPriceTrackings.Add(capitalPriceTrackingDto);

                        index = capitalPriceTrackings.Count - 1;
                    }


                    double deltaInventory = editedProduct.InputAmount - oldProduct.InputAmount;

                    var result = ChangeCapitalPriceTrackingsResultHelper.ChangeCapitalPriceTrackings(index,
                               deltaInventory, editedProduct.InputPrice, capitalPriceTrackings, false);

                    if (!setting.IsAllowNegativeInventoryBill && result.Inventory < 0)
                    {
                        throw new Exception("change_warehousing_make_negative_inventory");
                    }
                    // remove tracking record 
                    capitalPriceTrackings.RemoveAt(index);
                    // save new information
                    productStorage.Inventory = result.Inventory;
                    productStorage.CapitalPrice = result.CapitalPrice;
                    productStorage.CapitalPriceTrackings = result.CapitalPriceTrackingsJson;

                    _context.Update(productStorage);
                }
            }

            foreach (PropertyInfo propertyInfo in updationDto.GetType().GetProperties())
            {
                if (warehousing.GetType().GetProperty(propertyInfo.Name) != null
                    && propertyInfo.Name != "ProductList"
                    && propertyInfo.Name != "SupplierBillList"
                    && propertyInfo.Name != "InputDate"
                    )
                {
                    warehousing.GetType().GetProperty(propertyInfo.Name).SetValue(warehousing, propertyInfo.GetValue(updationDto, null));
                }
            }

            warehousing.SupplierBillList = JsonConvert.SerializeObject(updationDto.SupplierBillList);
            warehousing.UpdatedDateTime = DateTime.Now;

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            warehousing.UpdatedUserId = user.Id;
            warehousing.ProductList = JsonConvert.SerializeObject(updationDto.ProductList);


            warehousing.ProductMoney = productMoney;
            warehousing.SummaryMoney = warehousing.ProductMoney + warehousing.TaxMoney;
            warehousing.DebtMoney = warehousing.SummaryMoney - updationDto.PaymentMoney;

            _entity.Update(warehousing);

            var created = await _context.SaveChangesAsync();
            if (created < 1)
            {
                throw new InvalidOperationException("Database context could not create Warehousing.");
            }

            return warehousing.Id;
        }

        public async Task<bool> IsDuplicatedCode(string code)
        {
            var warehousing = await _entity.FirstOrDefaultAsync(w => w.Code == code);
            if (warehousing != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task _removeCapitalTracking(ProductForWareshousingCreationDto product, Guid storageId, Guid warehousingId)
        {
            var productStorages = await _context.ProductStorages.Where(ps => ps.StorageId == storageId && ps.ProductId == product.Id).ToListAsync();
            foreach (var productStorage in productStorages)
            {
                if (productStorage.Inventory >= product.InputAmount || setting.IsAllowNegativeInventoryBill)
                {
                    var capitalPriceTrackings = JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);
                    int index = capitalPriceTrackings.FindIndex(c => c.WarehousingId == warehousingId);
                    double deltaInventory = -1 * product.InputAmount;

                    var result = ChangeCapitalPriceTrackingsResultHelper.ChangeCapitalPriceTrackings(index,
                        deltaInventory, product.InputPrice, capitalPriceTrackings, true);

                    productStorage.Inventory = result.Inventory;
                    productStorage.CapitalPrice = result.CapitalPrice;
                    productStorage.CapitalPriceTrackings = result.CapitalPriceTrackingsJson;

                    // remove detail inventory
                    var oldProductProductionList = (productStorage.ProductProductionDateList == null) ? new List<ProductProductionDateDto>() : JsonConvert.DeserializeObject<List<ProductProductionDateDto>>(productStorage.ProductProductionDateList);
                    var detailInputAmountList = product.DetailInputAmountList;
                    foreach (var detailInputAmount in detailInputAmountList)
                    {
                        if (detailInputAmount.InputAmount > product.InputAmount)
                        {
                            throw new Exception("Detail InputAmount is greater than product InputAmount");
                        }

                        var oldProductProduction = oldProductProductionList.SingleOrDefault(p => p.ProductionWeekYear == detailInputAmount.ProductionWeekYear);
                        if (oldProductProduction != null)
                        {
                            oldProductProduction.Inventory -= detailInputAmount.InputAmount;
                        }
                        else
                        {
                            oldProductProductionList.Add(new ProductProductionDateDto()
                            {
                                ProductionWeekYear = detailInputAmount.ProductionWeekYear,
                                ProductionDate = DateTimeHelper.ConvertWeekYearToDateTime(detailInputAmount.ProductionWeekYear),
                                Inventory = detailInputAmount.InputAmount
                            });
                        }
                    }
                    productStorage.ProductProductionDateList = JsonConvert.SerializeObject(oldProductProductionList);

                }
                else
                {
                    throw new Exception("change_warehousing_make_negative_inventory");
                }
                _context.ProductStorages.Update(productStorage);

            }
        }

        public async Task<Guid> DestroyAsync(Guid id)
        {
            var warehousing = await _entity.SingleOrDefaultAsync(r => r.Id == id);
            if (warehousing == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }
            warehousing.IsActive = false;

            ProductForWareshousingCreationDto[] products = JsonConvert.DeserializeObject<ProductForWareshousingCreationDto[]>(warehousing.ProductList);

            foreach (ProductForWareshousingCreationDto product in products)
            {
                await _removeCapitalTracking(product, warehousing.StorageId, id);
            }
            var updated = await _context.SaveChangesAsync();

            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }

            return warehousing.Id;
        }

        public async Task<WarehousingReportDto> GetReport(DateTime fromDate, DateTime toDate, ICollection<Guid> storageIds)
        {
            var warehousingReport = new WarehousingReportDto
            {
                StorageWarehousingList = new List<ReturnStorageWarehousingDto>(),
                FromDate = fromDate,
                ToDate = toDate
            };

            foreach (var storageId in storageIds)
            {
                var storage = await _context.Storages.SingleAsync(s => s.Id == storageId);

                var warehousingList = await _entity.Where(b => b.StorageId == storageId
                && b.InputDate.Date >= fromDate.Date
                && b.InputDate.Date <= toDate.Date
                && b.IsActive == true
                ).Include(b => b.Supplier).Include(b => b.CreatedUser).Include(b => b.Storage).ToListAsync();

                warehousingReport.WarehousingAmount += warehousingList.Count;

                foreach (var warehousing in warehousingList)
                {
                    warehousingReport.ProductMoney += warehousing.ProductMoney;
                    warehousingReport.TaxMoney += warehousing.TaxMoney;
                    warehousingReport.SummaryMoney += warehousing.SummaryMoney;
                    warehousingReport.PaymentMoney += warehousing.PaymentMoney;
                    warehousingReport.DebtMoney += warehousing.DebtMoney;

                    ProductForWareshousingCreationDto[] products = JsonConvert.DeserializeObject<ProductForWareshousingCreationDto[]>(warehousing.ProductList);
                    foreach (var product in products)
                    {
                        warehousingReport.ProductAmount += product.InputAmount;
                    }
                }

                List<WarehousingDto> warehousingDtoList = Mapper.Map<List<WarehousingDto>>(warehousingList);
                for(int i = 0; i < warehousingList.Count; i++)
                {
                    warehousingDtoList[i].ProductList = JsonConvert.DeserializeObject
                        <List<ProductForWareshousingCreationDto>>(warehousingList[i].ProductList);
                }

                ReturnStorageWarehousingDto storageWarehousingDto = new ReturnStorageWarehousingDto
                {
                    StorageName = storage.Name,
                    WarehousingList = warehousingDtoList
                };
                warehousingReport.StorageWarehousingList.Add(storageWarehousingDto);
            }
            return warehousingReport;
        }

        public async Task<bool> IsDuplicatedSupplierBillCode(string code)
        {
            var warehousing = await _entity.SingleOrDefaultAsync(w => _isDuplicatedSupplierBillCode(w.SupplierBillList, code) == true);
            if (warehousing != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool _isDuplicatedSupplierBillCode(string supplierBillsJson, string code)
        {
            List<SupplierBillDto> supplierBills = JsonConvert.DeserializeObject<List<SupplierBillDto>>(supplierBillsJson);
            foreach (var supplierBill in supplierBills)
            {
                if (supplierBill.Code == code)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<List<WarehousingHistoryOfProductDto>> GetWarehousingHistory(Guid productId, Guid storageId, DateTime fromDate, DateTime toDate)
        {
            await Task.Delay(0);

            var warehousings = _entity.Where(b => b.StorageId == storageId
             && b.ProductList.Contains(productId.ToString()));

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                warehousings = warehousings.Where(w => w.CreatedDateTime.Date >= fromDate.Date
                        && w.CreatedDateTime.Date <= toDate.Date);
            }
            warehousings = warehousings.Include(w => w.Supplier);

            List<WarehousingHistoryOfProductDto> warehousingHistoryOfProducts = new List<WarehousingHistoryOfProductDto>();

            foreach (var warehousing in warehousings)
            {

                var productList = JsonConvert.DeserializeObject<List<ProductForWareshousingCreationDto>>(warehousing.ProductList);
                var product = productList.SingleOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    var warehousingHistoryOfProduct = new WarehousingHistoryOfProductDto()
                    {
                        CreatedDate = warehousing.CreatedDateTime,
                        Code = warehousing.Code,
                        Supplier = Mapper.Map<SupplierDto>(warehousing.Supplier),
                        Amount = product.InputAmount
                    };
                    warehousingHistoryOfProducts.Add(warehousingHistoryOfProduct);
                }
            }
            return warehousingHistoryOfProducts;

        }
    }

}

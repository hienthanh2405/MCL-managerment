using API.Entities;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using API.Infrastructure;
using API.Helpers;

namespace API.Services
{
    public class BillRepository : GenericRepository<BillEntity, BillDto, BillForCreationDto>, IBillRepository
    {
        private DatabaseContext _context;
        private DbSet<BillEntity> _entity;
        private DbSet<CustomerEntity> _customerEntity;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BillRepository(DatabaseContext context,
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor
            ) : base(context)
        {
            _context = context;
            _entity = _context.Set<BillEntity>();
            _customerEntity = _context.Set<CustomerEntity>();
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResults<BillDto>> GetListAsync(int offset, int limit, string keyword,
            SortOptions<BillDto, BillEntity> sortOptions, FilterOptions<BillDto, BillEntity> filterOptions,
            List<Guid> storageIds, DateTime fromDate, DateTime toDate

            )
        {

            IQueryable<BillEntity> query = _entity;
            query = sortOptions.Apply(query);
            query = filterOptions.Apply(query);


            if (keyword != null)
            {
                try
                {
                    DateTime searchDate = DateTime.ParseExact(keyword, "dd/MM/yyyy",
                                             System.Globalization.CultureInfo.InvariantCulture).Date;

                    query = _entity.Where(x =>
                       x.ProductList.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)

                      || x.AttachedDocuments.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Storage.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.CompanyName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.CompanyTaxCode.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.CompanyAddress.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.ShipAddress.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.ShipContactName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.ShipPhone.Contains(keyword, StringComparison.OrdinalIgnoreCase)

                      || x.Customer.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Customer.Phone.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Customer.FirstName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Customer.LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase)

                      || x.CreatedDateTime.Date == searchDate.Date
                    );

                }
                catch (Exception)
                {
                    query = _entity.Where(x =>
                       x.ProductList.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)

                      || x.AttachedDocuments.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Storage.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.CompanyName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.CompanyTaxCode.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.CompanyAddress.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.ShipAddress.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.ShipContactName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.ShipPhone.Contains(keyword, StringComparison.OrdinalIgnoreCase)

                      || x.Customer.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Customer.Phone.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Customer.FirstName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                      || x.Customer.LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    );
                }
            }

            query = query.Where(b => storageIds.Contains(b.StorageId));
            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                query = query.Where(b => b.CreatedDateTime.Date >= fromDate.Date
                && b.CreatedDateTime.Date <= toDate.Date);

            }
            else if (fromDate != DateTime.MinValue)
            {
                query = query.Where(b => b.CreatedDateTime.Date == fromDate.Date);
            }
            var totalSize = await query.CountAsync();

            var items = await query
                .Skip(offset * limit)
                .Take(limit)
                .Include(w => w.Storage)
                .Include(w => w.User)
                .Include(w => w.Customer)
                .ToArrayAsync();

            var returnItems = Mapper.Map<List<BillDto>>(items);

            for (var i = 0; i < items.Length; i++)
            {
                returnItems[i].ProductList = JsonConvert.DeserializeObject<List<ProductForBillCreationDto>>(items[i].ProductList);
            }
            return new PagedResults<BillDto>
            {
                Items = returnItems,
                TotalSize = totalSize
            };

        }

        public async new Task<BillForGetSingleDto> GetSingleAsync(Guid id)
        {
            var bill = await _entity.Include(b => b.Customer).Include(b => b.User).SingleOrDefaultAsync(r => r.Id == id);
            if (bill == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }
            var returnBill = Mapper.Map<BillForGetSingleDto>(bill);
            var productList = JsonConvert.DeserializeObject<List<ProductForBillGetSingleDto>>(bill.ProductList);
            foreach (var product in productList)
            {
                if (product.IsService)
                {
                    continue;
                }
                var productStorage = await _context.ProductStorages.SingleOrDefaultAsync(ps => ps.StorageId == bill.StorageId && ps.ProductId == product.Id);
                if (productStorage == null)
                {
                    throw new Exception("Can not find ProductStorage with  StorageId=" + bill.StorageId + ", productId=" + product.Id);
                }

                var productProductionDateList = (productStorage.ProductProductionDateList != null) ?
                    JsonConvert.DeserializeObject<List<ProductProductionDateDto>>(productStorage.ProductProductionDateList) :
                    new List<ProductProductionDateDto>();

                productProductionDateList = productProductionDateList.OrderBy(p => p.ProductionDate).ToList();
                var returnDetailAmountList = new List<ReturnDetailAmountDto>();
                foreach (var productProductionDate in productProductionDateList)
                {
                    var detailAmount = product.DetailAmountList.SingleOrDefault(p => p.ProductionWeekYear == productProductionDate.ProductionWeekYear);

                    var returnDetailAmount = new ReturnDetailAmountDto()
                    {
                        ProductionWeekYear = productProductionDate.ProductionWeekYear,
                        Inventory = productProductionDate.Inventory,
                        Amount = (detailAmount == null) ? 0 : detailAmount.Amount
                    };
                    returnDetailAmountList.Add(returnDetailAmount);
                }
                product.DetailAmountList = returnDetailAmountList;
            }
            returnBill.ProductList = productList;

            return returnBill;
        }


        new public async Task<Guid> CreateAsync(BillForCreationDto creationDto)
        {
            var customer = await _customerEntity.FirstOrDefaultAsync(x => x.Id == creationDto.CustomerId);
            if (customer == null)
            {
                throw new Exception("Can not find any customer with this id");
            }

            if (creationDto.UsedPoints > customer.AccumulationPoint)
            {
                throw new Exception("UsedPoints must be less or equal AccumulationPoint");
            }

            var newBill = new BillEntity();

            foreach (PropertyInfo propertyInfo in creationDto.GetType().GetProperties())
            {
                if (newBill.GetType().GetProperty(propertyInfo.Name) != null && propertyInfo.Name != "ProductList")
                {
                    newBill.GetType().GetProperty(propertyInfo.Name).SetValue(newBill, propertyInfo.GetValue(creationDto, null));
                }
            }

            var settings = _context.Settings.ToList();
            var defaultSetting = settings[0];
            // we have a lot of data to save!
            newBill.PointMoney = defaultSetting.PointToMoney * creationDto.UsedPoints;
            newBill.PointEarning = Math.Round(creationDto.TotalMoney / defaultSetting.MoneyToPoint, 1);
            newBill.CreatedDateTime = DateTimeHelper.GetVietnamNow();
            newBill.IsActive = true;

            newBill.ProductList = JsonConvert.SerializeObject(creationDto.ProductList);

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            newBill.UserId = user.Id;

            // generate the code
            bool isDuplicated = false;
            do
            {
                string lastestCode = _entity.Max(w => w.Code);
                newBill.Code = CodeGeneratorHelper.GetGeneratedCode(CONSTANT.BILL_PREFIX, lastestCode, CONSTANT.GENERATED_NUMBER_LENGTH);
                isDuplicated = await _isDuplicatedCode(newBill.Code);

            } while (isDuplicated);


            await _entity.AddAsync(newBill);

            // update accumulation points
            customer.AccumulationPoint += newBill.PointEarning - creationDto.UsedPoints;
            customer.AccumulationPoint = Math.Round(customer.AccumulationPoint, 1);
            if (creationDto.IsUpdatedCustomerShipping)
            {
                customer.ShipAddress = newBill.ShipAddress;
                customer.ShipContactName = newBill.ShipContactName;
                customer.ShipPhone = newBill.ShipPhone;
                customer.CompanyName = newBill.CompanyName;
                customer.CompanyAddress = newBill.CompanyAddress;
                customer.CompanyTaxCode = newBill.CompanyTaxCode;
            }

            _customerEntity.Update(customer);

            // update inventory for eact saled product
           
            foreach (ProductForBillCreationDto product in creationDto.ProductList)
            {
                if (product.IsService) // if product is service, we need not update anymore
                {
                    continue;
                }
                var productStorage = _context.ProductStorages.FirstOrDefault(p => p.ProductId == product.Id && p.StorageId == creationDto.StorageId);
                // if has a ProductStorage, we update inventory number.
                if (productStorage != null)
                {


                    if (product.Amount > productStorage.Inventory && !defaultSetting.IsAllowNegativeInventoryBill)
                    {
                        throw new Exception("amount_overcomes_inventory");
                    }
                    productStorage.Inventory -= product.Amount;
                    // update detail output amount

                    _addOrUpdateProductProductionDate(productStorage, product, true);

                    CapitalPriceTrackingDto capitalPriceTracking = new CapitalPriceTrackingDto
                    {
                        BillId = newBill.Id,
                        Amount = product.Amount,
                        CapitalPrice = productStorage.CapitalPrice,
                        Inventory = productStorage.Inventory
                    };
                    productStorage.CapitalPriceTrackings = productStorage.CapitalPriceTrackings ?? "[]";

                    var capitalPriceTrackings = JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);
                    capitalPriceTrackings.Add(capitalPriceTracking);
                    productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);

                    _context.ProductStorages.Update(productStorage);
                }
                else
                {
                    throw new Exception("ProductStorage is not exist");
                }
            }

            var created = await _context.SaveChangesAsync();
            if (created < 1)
            {
                throw new InvalidOperationException("Database context could not create data.");
            }
            return newBill.Id;
        }


        new public async Task<Guid> EditAsync(Guid id, BillForCreationDto updationDto)
        {
            var bill = await _entity.SingleOrDefaultAsync(r => r.Id == id);
            if (bill == null)
            {
                throw new Exception("Can not find object with this Id.");
            }
            if (!bill.IsActive)
            {
                throw new Exception("only_active_bill_can_edit");
            }

            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // update accumulation points 
            var customer = await _context.Customers.SingleOrDefaultAsync(c => c.Id == bill.CustomerId);
            if (customer == null)
            {
                throw new Exception("Can not find customer with id = " + bill.Id);
            }
            var settings = _context.Settings.ToList();
            var defaultSetting = settings[0];
            double newPointEarning = Math.Round(updationDto.TotalMoney / defaultSetting.MoneyToPoint, 1);
            customer.AccumulationPoint += bill.UsedPoints - bill.PointEarning - updationDto.UsedPoints + newPointEarning;
            customer.AccumulationPoint = Math.Round(customer.AccumulationPoint, 1);
            _context.Update(customer);

            bill.PointMoney = defaultSetting.PointToMoney * updationDto.UsedPoints;
            bill.PointEarning = newPointEarning;

            // Travel old product list. If not found each old product in the new product list, 
            //it means that the item is removed from the new list. 
            // We need to increase amount on ProductProductionDate, update the capital tracking list and update cost price.

            var oldProductList = JsonConvert.DeserializeObject<List<ProductForBillCreationDto>>(bill.ProductList);
            var newProductList = updationDto.ProductList;

            foreach (var oldProduct in oldProductList)
            {
                if (oldProduct.IsService)
                {
                    continue;
                }
                var productStorage = await _context.ProductStorages.SingleOrDefaultAsync(p => p.ProductId == oldProduct.Id && p.StorageId == updationDto.StorageId);
                var newProduct = newProductList.SingleOrDefault(p => p.Id == oldProduct.Id);
                if (newProduct == null) // the old product is removed on new product list
                {
                    _addOrUpdateProductProductionDate(productStorage, oldProduct, false);
                    _removeCapitalTrackingAndUpdateCapitalPriceInventory(productStorage, oldProduct, bill.Id);

                }
            }
            //Travel in new produt list.
            // If we do not find the old product, add a new trackingcapitalPrice and update the productProrductionDate.
            // If we can find the old product, we proceed to calculate the amount of difference.

            foreach (var newProduct in newProductList)
            {
                if (newProduct.IsService)
                {
                    continue;
                }
                var productStorage = await _context.ProductStorages.SingleOrDefaultAsync(p => p.ProductId == newProduct.Id && p.StorageId == updationDto.StorageId);
                var oldProduct = oldProductList.SingleOrDefault(p => p.Id == newProduct.Id);
                if (oldProduct == null)
                {
                    _addOrUpdateProductProductionDateWithAmount(productStorage, newProduct, oldProduct);
                    _updateCapitalTrackingCapitalPriceAndInventory(productStorage, newProduct, newProduct.Amount, bill.Id);
                }
                else
                {
                    _addOrUpdateProductProductionDateWithAmount(productStorage, newProduct, oldProduct);
                    _updateCapitalTrackingCapitalPriceAndInventory(productStorage, newProduct, newProduct.Amount, bill.Id);
                }
            }

            // update bill info

            foreach (PropertyInfo propertyInfo in updationDto.GetType().GetProperties())
            {
                // do not allow update Code ,CreatedDateTime
                string key = propertyInfo.Name;
                if (key != "Id"
                    && key != "Code"
                    && key != "CreatedDateTime"
                    && key != "ProductList"
                    && bill.GetType().GetProperty(propertyInfo.Name) != null
                )
                {
                    bill.GetType().GetProperty(key).SetValue(bill, propertyInfo.GetValue(updationDto, null));
                }
            }

            bill.ProductList = JsonConvert.SerializeObject(updationDto.ProductList);

            _entity.Update(bill);

            var updated = await _context.SaveChangesAsync();
            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }
            return id;
        }
        public async Task<Guid> DestroyAsync(Guid id)
        {
            var bill = await _entity.SingleOrDefaultAsync(r => r.Id == id);
            if (bill == null)
            {
                throw new Exception("Can not find object with this Id.");
            }
            if (!bill.IsActive)
            {
                throw new Exception("The bill has destroyed status. We can only destroy active bill.");
            }
            bill.IsActive = false;

            // descrease accumulation points
            var customer = await _context.Customers.SingleOrDefaultAsync(c => c.Id == bill.CustomerId);
            if (customer == null)
            {
                throw new Exception("Can not find customer with id=" + bill.CustomerId);
            }
            customer.AccumulationPoint = customer.AccumulationPoint - bill.PointEarning + bill.UsedPoints;
            _context.Customers.Update(customer);

            ProductForBillCreationDto[] products = JsonConvert.DeserializeObject<ProductForBillCreationDto[]>(bill.ProductList);

            foreach (var product in products)
            {
                if (product.IsService)
                {
                    continue;
                }
                var productStorages = await _context.ProductStorages.Where(ps => ps.StorageId == bill.StorageId && ps.ProductId == product.Id).ToListAsync();
                foreach (var productStorage in productStorages)
                {
                    // remove tracking record of capital 
                    var capitalPriceTrackings = JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);

                    int removedIndex = -1;

                    for (var i = 0; i < capitalPriceTrackings.Count; i++)
                    {
                        if (capitalPriceTrackings[i].BillId != Guid.Empty && capitalPriceTrackings[i].BillId == id)
                        {
                            removedIndex = i;
                            break;
                        }
                    }
                    if (removedIndex > -1)
                    {

                        var removedTracking = capitalPriceTrackings[removedIndex];

                        capitalPriceTrackings.RemoveAt(removedIndex);

                        for (var i = removedIndex; i < capitalPriceTrackings.Count; i++)
                        {
                            capitalPriceTrackings[i].Inventory = capitalPriceTrackings[i].Inventory + removedTracking.Amount;

                            if (capitalPriceTrackings[i].WarehousingId != Guid.Empty)
                            {
                                // calculate capital price after remove position for warehousing

                                capitalPriceTrackings[i].CapitalPrice =
                                  ProductStorageHelper.CalculateCapitalPrice(
                                  capitalPriceTrackings[i - 1].Inventory,
                                  capitalPriceTrackings[i - 1].CapitalPrice,
                                  capitalPriceTrackings[i].Amount,
                                  capitalPriceTrackings[i].InputPrice
                                  );
                            }
                        }
                    }

                    // calculate capital price again
                    productStorage.Inventory = capitalPriceTrackings[capitalPriceTrackings.Count - 1].Inventory;
                    productStorage.CapitalPrice = capitalPriceTrackings[capitalPriceTrackings.Count - 1].CapitalPrice;
                    productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);
                    // update detail output amount
                    _addOrUpdateProductProductionDate(productStorage, product, false);
                    _context.ProductStorages.Update(productStorage);
                }
            }

            await _context.SaveChangesAsync();
            return bill.Id;
        }

        public async Task<BillReportDto> GetReport(DateTime fromDate, DateTime toDate, ICollection<Guid> storageIds)
        {
            var billReport = new BillReportDto
            {
                StorageBillList = new List<ReturnStorageBillDto>(),
                FromDate = fromDate,
                ToDate = toDate
            };

            foreach (var storageId in storageIds)
            {
                var storage = await _context.Storages.SingleAsync(s => s.Id == storageId);

                var billList = await _entity.Where(b => b.StorageId == storageId
                && b.CreatedDateTime.Date >= fromDate.Date
                && b.CreatedDateTime.Date <= toDate.Date
                && b.IsActive == true
                ).Include(b => b.Customer).Include(b => b.User).ToListAsync();

                billReport.BillAmount += billList.Count;

                foreach (var bill in billList)
                {
                    if (bill.IsRetail)
                    {
                        billReport.RetailBillAmount++;
                    }
                    else
                    {
                        billReport.WholeSaleBillAmount++;
                    }
                    billReport.NoEditTotalMoney += bill.NoEditTotalMoney;
                    billReport.ShipMoney += bill.ShipMoney;
                    billReport.PointMoney += bill.PointMoney;
                    billReport.TotalMoney += bill.TotalMoney;
                    billReport.PaymentMoney += bill.PaymentMoney;
                    billReport.GuestDebtMoney += bill.GuestDebtMoney;
                    billReport.PaymentCash += bill.PaymentCash;
                    billReport.PaymentCard += bill.PaymentCard;
                    billReport.PaymentBank += bill.PaymentBank;
                    ProductForWareshousingUpdateDto[] products = JsonConvert.DeserializeObject<ProductForWareshousingUpdateDto[]>(bill.ProductList);
                    foreach (var product in products)
                    {
                        billReport.ProductAmount += product.Amount;
                    }

                }
                var billDtoList = new List<BillDto>();
                foreach (var bill in billList)
                {
                    var billDto = Mapper.Map<BillDto>(bill);
                    billDto.ProductList = JsonConvert.DeserializeObject<List<ProductForBillCreationDto>>(bill.ProductList);
                    billDtoList.Add(billDto);

                }
                ReturnStorageBillDto storageBillDto = new ReturnStorageBillDto
                {
                    StorageName = storage.Name,
                    BillList = billDtoList
                };
                billReport.StorageBillList.Add(storageBillDto);
            }
            return billReport;
        }

        private async Task<bool> _isDuplicatedCode(string code)
        {
            var bill = await _entity.FirstOrDefaultAsync(w => w.Code == code);
            if (bill != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void _updateCapitalTrackingCapitalPriceAndInventory(ProductStorageEntity productStorage, ProductForBillCreationDto product, double newAmount, Guid billId)
        {
            // update capital price with average
            if (productStorage.CapitalPriceTrackings == null)
            {
                throw new Exception("CapitalPriceTrackings is NULL with ProductStorage.Id=" + productStorage.Id);
            }
            var capitalPriceTrackings = JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);

            var startedIndex = capitalPriceTrackings.FindIndex(c => c.BillId == billId);
            if (startedIndex < 0) // product is new on bill, we need to create a new tracking
            {
                var newCapitalPriceTrackingDto = new CapitalPriceTrackingDto()
                {
                    BillId = billId,
                    Amount = newAmount,
                    CapitalPrice = productStorage.CapitalPrice,
                    Inventory = productStorage.Inventory - newAmount
                };
                capitalPriceTrackings.Add(newCapitalPriceTrackingDto);
            }
            else
            {
                capitalPriceTrackings[startedIndex].Amount = newAmount;

                for (int i = startedIndex; i < capitalPriceTrackings.Count; i++)
                {
                    if (capitalPriceTrackings[i].BillId != Guid.Empty) // this is a bill. we need to decrease the invetory
                    {
                        capitalPriceTrackings[i].Inventory = capitalPriceTrackings[i - 1].Inventory - capitalPriceTrackings[i].Amount;
                    }
                    else
                    {
                        capitalPriceTrackings[i].Inventory = capitalPriceTrackings[i - 1].Inventory + capitalPriceTrackings[i].Amount;
                    }

                    if (capitalPriceTrackings[i].BillId == Guid.Empty) // this is not a bill. we only update capital price for warehousing and inventory
                    {
                        capitalPriceTrackings[i].CapitalPrice =
                            ProductStorageHelper.CalculateCapitalPrice(
                            capitalPriceTrackings[i - 1].Inventory,
                            capitalPriceTrackings[i - 1].CapitalPrice,
                            capitalPriceTrackings[i].Amount,
                            capitalPriceTrackings[i].InputPrice
                            );
                    }
                    else
                    {
                        capitalPriceTrackings[i].CapitalPrice = capitalPriceTrackings[i - 1].CapitalPrice;
                    }

                }
            }

            productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);
            productStorage.CapitalPrice = capitalPriceTrackings[capitalPriceTrackings.Count - 1].CapitalPrice;
            productStorage.Inventory = capitalPriceTrackings[capitalPriceTrackings.Count - 1].Inventory;
            _context.ProductStorages.Update(productStorage);
        }

        private void _removeCapitalTrackingAndUpdateCapitalPriceInventory(ProductStorageEntity productStorage, ProductForBillCreationDto product, Guid billId)
        {
            // update capital price with average
            if (productStorage.CapitalPriceTrackings == null)
            {
                throw new Exception("CapitalPriceTrackings is NULL with ProductStorage.Id=" + productStorage.Id);
            }
            var capitalPriceTrackings = JsonConvert.DeserializeObject<List<CapitalPriceTrackingDto>>(productStorage.CapitalPriceTrackings);

            var startedIndex = capitalPriceTrackings.FindIndex(c => c.BillId == billId);
            capitalPriceTrackings.RemoveAt(startedIndex);

            for (int i = startedIndex; i < capitalPriceTrackings.Count; i++)
            {
                capitalPriceTrackings[i].Inventory = capitalPriceTrackings[i - 1].Inventory + capitalPriceTrackings[i].Amount;

                capitalPriceTrackings[i].CapitalPrice =
                         ProductStorageHelper.CalculateCapitalPrice(
                         capitalPriceTrackings[i - 1].Inventory,
                         capitalPriceTrackings[i - 1].CapitalPrice,
                         capitalPriceTrackings[i].Amount,
                         capitalPriceTrackings[i].InputPrice
                         );
            }

            productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);
            productStorage.CapitalPrice = capitalPriceTrackings[capitalPriceTrackings.Count - 1].CapitalPrice;
            productStorage.Inventory = capitalPriceTrackings[capitalPriceTrackings.Count - 1].Inventory;

            _context.ProductStorages.Update(productStorage);
        }

        private void _addOrUpdateProductProductionDate(ProductStorageEntity productStorage, ProductForBillCreationDto product, bool isDecreaseInventory)
        {
            if (product.DetailAmountList != null && product.DetailAmountList.Count > 0)
            {
                var productProductionDateList = JsonConvert.DeserializeObject<List<ProductProductionDateDto>>(productStorage.ProductProductionDateList);

                foreach (var detailAmount in product.DetailAmountList)
                {
                    var productProductionDate = productProductionDateList.SingleOrDefault(p => p.ProductionWeekYear == detailAmount.ProductionWeekYear);
                    if (productProductionDate == null)
                    {
                        // add new product production date 
                        var newProductProductionDate = new ProductProductionDateDto()
                        {
                            ProductionWeekYear = detailAmount.ProductionWeekYear,
                            ProductionDate = DateTimeHelper.ConvertWeekYearToDateTime(detailAmount.ProductionWeekYear),
                            Inventory = 0 - detailAmount.Amount
                        };
                        productProductionDateList.Add(newProductProductionDate);

                    }
                    else
                    {
                        if (isDecreaseInventory)
                        {
                            productProductionDate.Inventory -= detailAmount.Amount;
                        }
                        else
                        {
                            productProductionDate.Inventory += detailAmount.Amount;
                        }
                    }


                    productStorage.ProductProductionDateList = JsonConvert.SerializeObject(productProductionDateList);
                }
                _context.ProductStorages.Update(productStorage);

            }

        }

        private void _addOrUpdateProductProductionDateWithAmount(ProductStorageEntity productStorage, ProductForBillCreationDto newProduct, ProductForBillCreationDto oldProduct)
        {
            if (newProduct.DetailAmountList != null && newProduct.DetailAmountList.Count > 0)
            {
                var oldProductProductionDateList = JsonConvert.DeserializeObject<List<ProductProductionDateDto>>(productStorage.ProductProductionDateList);

                foreach (var newProductDetailAmount in newProduct.DetailAmountList)
                {
                    if (newProductDetailAmount.Amount <= 0)
                    {
                        continue;
                    }

                    var oldProductProductionDate = oldProductProductionDateList.SingleOrDefault(p => p.ProductionWeekYear == newProductDetailAmount.ProductionWeekYear);
                    if (oldProductProductionDate == null)
                    {
                        // add new product production date 
                        var newProductProductionDate = new ProductProductionDateDto()
                        {
                            ProductionWeekYear = newProductDetailAmount.ProductionWeekYear,
                            ProductionDate = DateTimeHelper.ConvertWeekYearToDateTime(newProductDetailAmount.ProductionWeekYear),
                            Inventory = 0 - newProductDetailAmount.Amount
                        };
                        oldProductProductionDateList.Add(newProductProductionDate);

                    }
                    else
                    {
                        double oldAmount = 0;
                        if (oldProduct != null)
                        {
                            var oldDetailAmount = oldProduct.DetailAmountList.SingleOrDefault(d => d.ProductionWeekYear == newProductDetailAmount.ProductionWeekYear);
                            oldAmount = oldDetailAmount.Amount;
                        }

                        oldProductProductionDate.Inventory += oldAmount - newProductDetailAmount.Amount;
                    }

                    productStorage.ProductProductionDateList = JsonConvert.SerializeObject(oldProductProductionDateList);
                }
                _context.ProductStorages.Update(productStorage);

            }

        }

        public async Task<List<BillHistoryOfProductDto>> GetBillHistory(Guid productId, Guid storageId, DateTime fromDate, DateTime toDate)
        {
            await Task.Delay(0);

            var bills = _entity.Where(b => b.StorageId == storageId
             && b.ProductList.Contains(productId.ToString()));

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                bills = bills.Where(b => b.CreatedDateTime.Date >= fromDate.Date
                        && b.CreatedDateTime.Date <= toDate.Date);
            }
            bills = bills.Include(b => b.Customer);

            List<BillHistoryOfProductDto> billHistoryOfProducts = new List<BillHistoryOfProductDto>();

            foreach (var bill in bills)
            {

                var productList = JsonConvert.DeserializeObject<List<ProductForBillCreationDto>>(bill.ProductList);
                var product = productList.SingleOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    var billHistoryOfProduct = new BillHistoryOfProductDto()
                    {
                        CreatedDate = bill.CreatedDateTime,
                        Code = bill.Code,
                        Customer = Mapper.Map<CustomerDto>(bill.Customer),
                        Amount = product.Amount
                    };
                    billHistoryOfProducts.Add(billHistoryOfProduct);
                }
            }
            return billHistoryOfProducts;

        }


    }

}
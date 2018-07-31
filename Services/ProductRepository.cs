using API.Entities;
using API.Helpers;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using API.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using API.Controllers;

namespace API.Services
{
    public class ProductRepository : CodeGenericRepository<ProductEntity, ProductDto, ProductForCreationDto>, IProductRepository
    {
        private DatabaseContext _context;
        private DbSet<ProductEntity> _entity;
        private List<ProductStorageEntity> _allProductStorages;

        public ProductRepository(DatabaseContext context) : base(context)
        {
            _context = context;
            _entity = _context.Set<ProductEntity>();
            _allProductStorages = _context.ProductStorages.Include(ps => ps.Storage).ToList();
        }

        public async Task<ProductPagedResultDto> GetListAsync(int offset, int limit, string keyword, Guid currentStorageId, SortOptions<ProductDto, ProductEntity> sortOptions,
        FilterOptions<ProductDto, ProductEntity> filterOptions, bool isOldProduct)
        {
            IQueryable<ProductEntity> query = _entity;
            query = sortOptions.Apply(query);
            query = filterOptions.Apply(query);

            if (keyword != null)
            {
                query = query.Where(p =>
                 (p.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || p.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || p.ProductCategory.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || p.Provider.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                );
            }

            var size = await query.CountAsync();

            var productEntities = await query.Include(p => p.ProductCategory)
                .Include(p => p.Provider)
                .Skip(offset * limit)
                .Take(limit)
                .ToListAsync();

            var productDtos = new List<ProductDto>();

            // convert string UnitConventerList to List<UnitConverterDto>
            foreach (var productEntity in productEntities)
            {
                var productDto = Mapper.Map<ProductDto>(productEntity);
                productDto.UnitConverterList = (productEntity.UnitConverterList == null) ?
                    new List<UnitConverterDto>() : JsonConvert.DeserializeObject<List<UnitConverterDto>>(productEntity.UnitConverterList);

                productDtos.Add(productDto);
            }


            DateTime today = DateTime.Now;
            var settings = _context.Settings.ToList();
            var setting = settings[0];
            var oldProducts = new List<ProductDto>();

            foreach (var product in productDtos)
            {
                //var productStorage = await _context.ProductStorages
                //    .Include(ps => ps.Storage)
                //    .SingleOrDefaultAsync(ps => ps.StorageId == currentStorageId && ps.ProductId == product.Id);

                var productStorage = _allProductStorages
                    .SingleOrDefault(ps => ps.StorageId == currentStorageId && ps.ProductId == product.Id);

                if (productStorage == null)
                {
                    throw new Exception("Can not find ProductStorage with productId=" + product.Id + ", StorageId=" + currentStorageId);
                }
                // find the lowest discount

                product.Inventory = productStorage.Inventory;
                product.CapitalPrice = productStorage.CapitalPrice;
                product.Storage = Mapper.Map<StorageDto>(productStorage.Storage);
                product.PlacePosition = productStorage.PlacePosition;
                product.StorageName = product.Storage.Name;

                if (productStorage.ProductProductionDateList != null && productStorage.ProductProductionDateList.Length > 0)
                {
                    product.ProductProductionDateList = JsonConvert.DeserializeObject<List<ProductProductionDateDto>>(productStorage.ProductProductionDateList);
                    product.ProductProductionDateList = product.ProductProductionDateList.OrderBy(p => p.ProductionDate).ToList();

                    foreach (var productProductionDate in product.ProductProductionDateList)
                    {
                        int comparedDays = setting.OldProductWarmingDays;
                        if (product.ProductCategory.Keyword == CONSTANT.PRODUCT_CATEGORY_TRAVEL_TIRE)
                        {
                            comparedDays = setting.OldTravelTireWarmingDays;
                        }
                        else if (product.ProductCategory.Keyword == CONSTANT.PRODUCT_CATEGORY_TRUCK_TIRE)
                        {
                            comparedDays = setting.OldTruckTireWarmingDays;
                        }

                        if ((today.Date - productProductionDate.ProductionDate).TotalDays >= comparedDays)
                        {
                            oldProducts.Add(product);
                            break;
                        }
                    }

                }
                else
                {
                    product.ProductProductionDateList = new List<ProductProductionDateDto>();
                }

            }

            //var returnProducts = (isOldProduct) ? oldProducts : productDtos;

            //returnProducts = returnProducts.Skip(offset * limit).Take(limit).ToList();

            return new ProductPagedResultDto
            {
                Items = productDtos,
                TotalSize = size,
                OldProductNumber = oldProducts.Count
            };
        }

        new public async Task<Guid> CreateAsync(ProductForCreationDto creationDto)
        {

            var newProduct = new ProductEntity();

            var existedProduct = await _entity.SingleOrDefaultAsync(p => p.Code == creationDto.Code);
            if (existedProduct != null)
            {
                throw new InvalidOperationException("Product has Code which is existed on database");
            }

            foreach (PropertyInfo propertyInfo in creationDto.GetType().GetProperties())
            {
                if (newProduct.GetType().GetProperty(propertyInfo.Name) != null
                    && propertyInfo.Name != "UnitConverterList")
                {
                    newProduct.GetType().GetProperty(propertyInfo.Name).SetValue(newProduct, propertyInfo.GetValue(creationDto, null));
                }
            }
            newProduct.UnitConverterList = (creationDto.UnitConverterList == null) ? null : JsonConvert.SerializeObject(creationDto.UnitConverterList);
            newProduct.UpdatePriceDate = DateTime.Now;

            await _entity.AddAsync(newProduct);

            var storages = _context.Storages.ToArray();
            foreach (var storage in storages)
            {
                var productStorage = new ProductStorageEntity
                {
                    ProductId = newProduct.Id,
                    StorageId = storage.Id,
                    Inventory = 0
                };

                if (storage.Id == creationDto.CurrentStorageId)
                {
                    productStorage.Inventory = creationDto.Inventory;
                    productStorage.PlacePosition = creationDto.PlacePosition;
                    productStorage.CapitalPrice = creationDto.CapitalPrice;
                    if (creationDto.ProductProductionDateList != null)
                    {
                        foreach (var productProductionDate in creationDto.ProductProductionDateList)
                        {
                            if (productProductionDate.ProductionWeekYear != null)
                            {
                                DateTime productionDate = DateTimeHelper.ConvertWeekYearToDateTime(productProductionDate.ProductionWeekYear);
                                productProductionDate.ProductionDate = productionDate;
                            }
                        }
                        productStorage.ProductProductionDateList = JsonConvert.SerializeObject(creationDto.ProductProductionDateList);
                    }


                    // create tracking capital price 
                    CapitalPriceTrackingDto capitalPriceTracking = new CapitalPriceTrackingDto
                    {
                        Amount = creationDto.Inventory,
                        InputPrice = creationDto.CapitalPrice,
                        CapitalPrice = creationDto.CapitalPrice,
                        Inventory = creationDto.Inventory
                    };
                    var capitalPriceTrackings = new List<CapitalPriceTrackingDto>
                    {
                        capitalPriceTracking
                    };

                    productStorage.CapitalPriceTrackings = JsonConvert.SerializeObject(capitalPriceTrackings);

                }
                await _context.ProductStorages.AddAsync(productStorage);
            }

            var created = await _context.SaveChangesAsync();
            if (created < 1)
            {
                throw new InvalidOperationException("Database context could not create data.");
            }
            return newProduct.Id;
        }

        new public async Task<Guid> EditAsync(Guid id, ProductForCreationDto updationDto)
        {

            var updatedProduct = await _entity.SingleOrDefaultAsync(r => r.Id == id);
            if (updatedProduct == null)
            {
                throw new InvalidOperationException("Can not find object with this Id.");
            }

            var existedProduct = _entity.FirstOrDefault(p => p.Code == updationDto.Code && p.Id != id);
            if (existedProduct != null)
            {
                throw new InvalidOperationException("Product has Code which is existed on database: " + existedProduct.Code);
            }

            if (updatedProduct.SalePrice != updationDto.SalePrice
                || updatedProduct.WholeSalePrice != updationDto.WholeSalePrice)
            {
                updatedProduct.UpdatePriceDate = DateTime.Now;
            }

            foreach (PropertyInfo propertyInfo in updationDto.GetType().GetProperties())
            {
                if (updatedProduct.GetType().GetProperty(propertyInfo.Name) != null && propertyInfo.Name != "UnitConverterList" && propertyInfo.Name != "Id")
                {
                    updatedProduct.GetType().GetProperty(propertyInfo.Name).SetValue(updatedProduct, propertyInfo.GetValue(updationDto, null));
                }
            }
            updatedProduct.UnitConverterList = (updationDto.UnitConverterList == null) ? null : JsonConvert.SerializeObject(updationDto.UnitConverterList);

            _entity.Update(updatedProduct);

            var productStorage = await _context.ProductStorages
                    .SingleOrDefaultAsync(ps => ps.StorageId == updationDto.CurrentStorageId && ps.ProductId == id);
            if (productStorage == null)
            {
                throw new Exception("Can not find ProductStorage with productId=" + id + ", StorageId=" + updationDto.CurrentStorageId);
            }

            // should not allow for update CapitalPrice & Inventory when updating a product
            // but we have to allow right now to import product from excel
            productStorage.CapitalPrice = updationDto.CapitalPrice;
            productStorage.Inventory = updationDto.Inventory;
            productStorage.PlacePosition = updationDto.PlacePosition;

            if (updationDto.ProductProductionDateList != null)
            {
                foreach (var productProductionDate in updationDto.ProductProductionDateList)
                {
                    if (productProductionDate.ProductionWeekYear != null)
                    {
                        DateTime productionDate = DateTimeHelper.ConvertWeekYearToDateTime(productProductionDate.ProductionWeekYear);
                        productProductionDate.ProductionDate = productionDate;

                        productStorage.ProductProductionDateList = JsonConvert.SerializeObject(updationDto.ProductProductionDateList);
                    }

                }
            }


            _context.ProductStorages.Update(productStorage);

            var updated = await _context.SaveChangesAsync();

            if (updated < 1)
            {
                throw new InvalidOperationException("Database context could not update data.");
            }
            return updatedProduct.Id;
        }

        public async Task<ProductImportForReturnDto> ImportAsync(string productList)
        {
            ProductForImportDto[] products = JsonConvert.DeserializeObject<ProductForImportDto[]>(productList);
            ProductImportForReturnDto productImportForReturn = new ProductImportForReturnDto
            {
                TotalNumber = products.Length,
                NotFoundItemCodes = new List<string>(),
                ErrorItemCodes = new List<string>()
            };

            foreach (var product in products)
            {
                try
                {
                    var currentProduct = await _entity.SingleOrDefaultAsync(p => p.Code == product.Code);
                    if (currentProduct != null)
                    {
                        if (currentProduct.SalePrice != product.SalePrice
                         || currentProduct.WholeSalePrice != product.WholeSalePrice)
                        {
                            currentProduct.UpdatePriceDate = DateTime.Now;
                        }
                        currentProduct.Name = product.Name;

                        currentProduct.WholeSalePrice = product.WholeSalePrice;
                        currentProduct.SalePrice = product.SalePrice;

                        _entity.Update(currentProduct);

                        var updated = await _context.SaveChangesAsync();

                        if (updated < 1)
                        {
                            productImportForReturn.ErrorNumber++;
                            productImportForReturn.ErrorItemCodes.Add(product.Code);
                        }
                        else
                        {
                            productImportForReturn.SuccessNumber++;
                        }
                    }
                    else
                    {
                        productImportForReturn.NotFoundNumber++;
                        productImportForReturn.NotFoundItemCodes.Add(product.Code);
                    }
                }
                catch (Exception)
                {
                    productImportForReturn.ErrorNumber++;
                    productImportForReturn.ErrorItemCodes.Add(product.Code);
                }
            }
            return productImportForReturn;

        }


        public async Task<bool> ImportToCreateAsync(List<ProductForCreateImportDto> productList)
        {
            var defaultProvider = await _context.Providers.SingleOrDefaultAsync(p => p.Name == "Chưa phân loại");
            var bsProvider = await _context.Providers.SingleOrDefaultAsync(p => p.Name == "Bridgestone");
            var micProvider = await _context.Providers.SingleOrDefaultAsync(p => p.Name == "Michelin");
            var fuchProvider = await _context.Providers.SingleOrDefaultAsync(p => p.Name == "Fuchs");

            foreach (var product in productList)
            {
                ProductForCreationDto productForCreationDto = Mapper.Map<ProductForCreationDto>(product);
                var currentStorage = await _context.Storages.SingleOrDefaultAsync(s => s.Name == product.CurrentStorageName);
                productForCreationDto.CurrentStorageId = currentStorage.Id;
                if (productForCreationDto.Code.Substring(0, 2) == "BS")
                {
                    productForCreationDto.ProviderId = bsProvider.Id;
                }
                else if (productForCreationDto.Code.Substring(0, 3) == "MIC")
                {
                    productForCreationDto.ProviderId = micProvider.Id;
                }
                else if (productForCreationDto.Code.Contains("Fuchs"))
                {
                    productForCreationDto.ProviderId = fuchProvider.Id;
                }
                else
                {
                    productForCreationDto.ProviderId = defaultProvider.Id;
                }
                var productCategory = await _context.ProductCategories.SingleOrDefaultAsync(p => p.Name == product.ProductCategoryName);


                if (productCategory == null)
                {
                    var newProductCategory = new ProductCategoryEntity()
                    {
                        Name = product.ProductCategoryName,
                        Keyword = product.ProductCategoryName.ToUpper()
                    };
                    productForCreationDto.ProductCategoryId = newProductCategory.Id;
                }
                else
                {
                    productForCreationDto.ProductCategoryId = productCategory.Id;
                }

                productForCreationDto.ProductProductionDateList = new List<ProductProductionDateDto>
                {
                    new ProductProductionDateDto
                    {
                        ProductionWeekYear = _convertDto4CharsTo6Chars( product.Dot),
                        Inventory = product.Inventory
                    }
                };

                var existedProduct = await _context.Products.SingleOrDefaultAsync(p => p.Code == productForCreationDto.Code);
                if (existedProduct == null)
                {
                    await CreateAsync(productForCreationDto);
                }
                else
                {
                    await EditAsync(existedProduct.Id, productForCreationDto);
                }
            }
            return true;
        }

        private string _convertDto4CharsTo6Chars(string fourcharsDot)
        {
            string[] stringSeparators = new string[] { "/" };
            var result = fourcharsDot.Split(stringSeparators, StringSplitOptions.None);
            return result[0] + "/20" + result[1];
        }

        public async Task<bool> IsDuplicatedName(Guid id, string name)
        {
            var entity = await _entity.SingleOrDefaultAsync(r => r.Name == name && r.Id != id);
            if (entity == null)
            {
                return false;
            }
            return true;
        }

        public async Task<PagedResults<ProductDto>> GetProductListByCategoryIdsAsync(List<Guid> productCategoryIds)
        {
            await Task.Delay(0);
            List<ProductEntity> productEntites = new List<ProductEntity>();

            foreach (Guid productCategoryId in productCategoryIds)
            {
                var products = _entity.Where(p => p.ProductCategoryId == productCategoryId);
                productEntites.AddRange(products);
            }

            return new PagedResults<ProductDto>()
            {
                Items = Mapper.Map<List<ProductDto>>(productEntites),
                TotalSize = productEntites.Count
            };

        }

    }
}

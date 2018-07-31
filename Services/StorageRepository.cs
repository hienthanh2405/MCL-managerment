using API.Entities;
using API.Helpers;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public class StorageRepository : GenericRepository<StorageEntity, StorageDto, StorageDto>, IStorageRepository
    {
        private DatabaseContext _context;
        private DbSet<StorageEntity> _entity;
        private readonly IUserRepository _userRepository;

        public StorageRepository(DatabaseContext context , IUserRepository userRepository) : base(context)
        {
            _context = context;
            _entity = _context.Set<StorageEntity>();
            _userRepository = userRepository;
        }


        new public async Task<Guid> CreateAsync(StorageDto storageDto)
        {
            StorageEntity newStorage = new StorageEntity()
            {
                Name = storageDto.Name,
                Address = storageDto.Address
            };

            await _entity.AddAsync(newStorage);
            // we need to generate product storages for all product 
            var products = await _context.Products.ToListAsync();
            foreach(var product in products)
            {
                var productStorage = new ProductStorageEntity()
                {
                    ProductId = product.Id,
                    StorageId = newStorage.Id,
                    ProductProductionDateList = null,
                    Inventory = 0,
                    CapitalPrice = 0,
                    PlacePosition = null,
                    CapitalPriceTrackings = null
                };
                await _context.AddAsync(productStorage);
            }
            // we need to create user storage for all owners

            var owners = await _userRepository.GetUsersWithRoleNameAsync(CONSTANT.ROLE_OWNER);

            foreach(var owner in owners)
            {
                var userStorage = new UserStorageEntity()
                {
                    StorageId = newStorage.Id,
                    UserId = owner.Id
                };
                _context.UserStorages.Add(userStorage);
            };

            var created = await _context.SaveChangesAsync();
            if (created < 1)
            {
                throw new InvalidOperationException("Database context could not create data.");
            }
            return newStorage.Id;
        }
    }
}

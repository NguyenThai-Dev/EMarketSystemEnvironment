using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.CustomerModule.DTOs;
using EMarket.Modules.CustomerModule.Services.Interfaces;

namespace EMarket.Modules.CustomerModule.Services.Implementations
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly EMarket_DBEntities _db;

        public CustomerAddressService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<List<CustomerAddressDTO>> GetCustomerAddressAsync(int customerId)
        {
            return await _db.CustomerAddresses
                .Where(x => x.customer_id == customerId)
                .Select(x => new CustomerAddressDTO
                {
                    AddressId = x.address_id,
                    CustomerId = x.customer_id,
                    FullAddress = x.full_address,
                    AddressUrl = x.address_url,
                    IsDefault = x.is_default ?? true
                })
                .ToListAsync();
        }

        public async Task<CustomerAddressDTO> GetDefaultCustomerAddressAsync(int customerId)
        {
            var entity = await _db.CustomerAddresses
                .Where(x => x.customer_id == customerId && (x.is_default ?? true))
                .FirstOrDefaultAsync();
            if (entity == null) return null;
            return new CustomerAddressDTO
            {
                AddressId = entity.address_id,
                CustomerId = entity.customer_id,
                FullAddress = entity.full_address,
                AddressUrl = entity.address_url,
                IsDefault = entity.is_default ?? true
            };
        }

        public async Task<CustomerAddressDTO> GetCustomerAddressByIdAsync(int id)
        {
            var entity = await _db.CustomerAddresses.FindAsync(id);
            if (entity == null) return null;
            return new CustomerAddressDTO
            {
                AddressId = entity.address_id,
                CustomerId = entity.customer_id,
                FullAddress = entity.full_address,
                AddressUrl = entity.address_url,
                IsDefault = entity.is_default ?? true
            };
        }

        public async Task<bool> CreateCustomerAddressAsync(CustomerAddressCreateDTO dto)
        {
            var entity = new CustomerAddress
            {
                customer_id = dto.CustomerId,
                full_address = dto.FullAddress,
                address_url = dto.AddressUrl,
                is_default = dto.IsDefault,
                created_at = DateTime.Now
            };

            _db.CustomerAddresses.Add(entity);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCustomerAddressAsync(CustomerAddressUpdateDTO dto)
        {
            var entity = await _db.CustomerAddresses.FindAsync(dto.AddressId);
            if (entity == null) return false;

            entity.full_address = dto.FullAddress;
            entity.address_url = dto.AddressUrl;
            entity.is_default = dto.IsDefault;
            entity.updated_at = DateTime.Now;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCustomerAddressAsync(int id)
        {
            var entity = await _db.CustomerAddresses.FindAsync(id);
            if (entity == null) return false;

            _db.CustomerAddresses.Remove(entity);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
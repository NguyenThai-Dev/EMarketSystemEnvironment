using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.ProductModule.DTOs;
using EMarket.Modules.ProductModule.Services.Interfaces;

namespace EMarket.Modules.ProductModule.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly EMarket_DBEntities _db;

        public SupplierService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<List<SupplierDTO>> GetAllSupplierAsync()
        {
            try
            {
                var data = await _db.Suppliers
                    .OrderByDescending(x => x.supplier_id)
                    .Select(x => new SupplierDTO
                    {
                        SupplierId = x.supplier_id,
                        Name = x.name,
                        Email = x.email,
                        Phone = x.phone,
                        Address = x.address,
                        AddressUrl = x.address_url,
                        ContactPerson = x.contact_person
                    })
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách nhà cung cấp: " + ex.Message, ex);
            }
        }

        public async Task<List<SupplierDTO>> GetFilteredSupplierAsync(string name)
        {
            try
            {
                var query = _db.Suppliers.AsQueryable();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(x => x.name.Contains(name));
                }

                var data = await query
                    .OrderByDescending(x => x.supplier_id)
                     .Select(x => new SupplierDTO
                     {
                         SupplierId = x.supplier_id,
                         Name = x.name,
                         Email = x.email,
                         Phone = x.phone,
                         Address = x.address,
                         AddressUrl = x.address_url,
                         ContactPerson = x.contact_person,

                         CanBeDeleted = !_db.Products.Any(p => p.supplier_id == x.supplier_id)
                     })
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách loại sản phẩm: " + ex.Message, ex);
            }
        }

        public async Task<List<SupplierDTO>> GetAllSupplierByIdAsync(List<int> ids)
        {
            try
            {
                var data = await _db.Suppliers
                    .Where(x => ids.Contains(x.supplier_id))
                    .OrderByDescending(x => x.supplier_id)
                    .Select(x => new SupplierDTO
                    {
                        SupplierId = x.supplier_id,
                        Name = x.name,
                        Email = x.email,
                        Phone = x.phone,
                        Address = x.address,
                        AddressUrl = x.address_url,
                        ContactPerson = x.contact_person
                    })
                    .ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách nhà cung cấp theo ID: " + ex.Message, ex);
            }
        }

        public async Task<SupplierDTO> GetSupplierByIdAsync(int id)
        {
            try
            {
                var entity = await _db.Suppliers.FindAsync(id);
                if (entity == null)
                    return null;

                return new SupplierDTO
                {
                    SupplierId = entity.supplier_id,
                    Name = entity.name,
                    Email = entity.email,
                    Phone = entity.phone,
                    Address = entity.address,
                    AddressUrl = entity.address_url,
                    ContactPerson = entity.contact_person
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy nhà cung cấp theo ID: " + ex.Message, ex);
            }
        }

        public async Task<bool> CreateSupplierAsync(SupplierDTO dto)
        {
            try
            {
                var entity = new Supplier
                {
                    name = dto.Name,
                    email = dto.Email,
                    phone = dto.Phone,
                    address = dto.Address,
                    address_url = dto.AddressUrl,
                    contact_person = dto.ContactPerson
                };

                _db.Suppliers.Add(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tạo mới nhà cung cấp: " + ex.Message, ex);
            }
        }

        public async Task<bool> UpdateSupplierAsync(SupplierDTO dto)
        {
            try
            {
                var entity = await _db.Suppliers.FindAsync(dto.SupplierId);
                if (entity == null)
                    return false;

                entity.name = dto.Name;
                entity.email = dto.Email;
                entity.phone = dto.Phone;
                entity.address = dto.Address;
                entity.address_url = dto.AddressUrl;
                entity.contact_person = dto.ContactPerson;

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật nhà cung cấp: " + ex.Message, ex);
            }
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            try
            {
                var entity = await _db.Suppliers.FindAsync(id);
                if (entity == null)
                    return false;

                _db.Suppliers.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa nhà cung cấp: " + ex.Message, ex);
            }
        }
    }
}

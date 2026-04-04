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
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly EMarket_DBEntities _db;

        public ProductCategoryService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<List<ProductCategoryDTO>> GetAllProductCategoryAsync()
        {
            try
            {
                var data = await _db.ProductCategories
                    .OrderByDescending(x => x.category_id)
                    .Select(x => new ProductCategoryDTO
                    {
                        CategoryId = x.category_id,
                        Name = x.name,
                        Description = x.description
                    })
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách loại sản phẩm: " + ex.Message, ex);
            }
        }

        public async Task<ProductCategoryDTO> GetProductCategoryByIdAsync(int id)
        {
            try
            {
                var entity = await _db.ProductCategories.FindAsync(id);
                if (entity == null)
                    return null;

                return new ProductCategoryDTO
                {
                    CategoryId = entity.category_id,
                    Name = entity.name,
                    Description = entity.description
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy loại sản phẩm theo ID: " + ex.Message, ex);
            }
        }

        public async Task<List<ProductCategoryDTO>> GetFilteredProductCategoriesAsync(string name)
        {
            try
            {
                var query = _db.ProductCategories.AsQueryable();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(x => x.name.Contains(name));
                }

                var data = await query
                    .OrderByDescending(x => x.category_id)
                    .Select(x => new ProductCategoryDTO
                    {
                        CategoryId = x.category_id,
                        Name = x.name,
                        Description = x.description,

                        CanBeDeleted = !_db.Products.Any(p => p.category_id == x.category_id)
                    })
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách loại sản phẩm: " + ex.Message, ex);
            }
        }

        public async Task<bool> CreateProductCategoryAsync(ProductCategoryDTO dto)
        {
            try
            {
                var entity = new ProductCategory
                {
                    name = dto.Name,
                    description = dto.Description
                };

                _db.ProductCategories.Add(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tạo mới loại sản phẩm: " + ex.Message, ex);
            }
        }

        public async Task<bool> UpdateProductCategoryAsync(ProductCategoryDTO dto)
        {
            try
            {
                var entity = await _db.ProductCategories.FindAsync(dto.CategoryId);
                if (entity == null)
                    return false;

                entity.name = dto.Name;
                entity.description = dto.Description;

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật loại sản phẩm: " + ex.Message, ex);
            }
        }

        public async Task<bool> DeleteProductCategoryAsync(int id)
        {
            try
            {
                var entity = await _db.ProductCategories.FindAsync(id);
                if (entity == null)
                    return false;

                _db.ProductCategories.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa loại sản phẩm: " + ex.Message, ex);
            }
        }

        public async Task<Dictionary<int, string>> GetCategoriesByIdsAsync(List<int> ids)
        {
            try
            {
                var categories = await _db.ProductCategories
                    .Where(c => ids.Contains(c.category_id))
                    .Select(c => new { c.category_id, c.name })
                    .ToListAsync();
                return categories.ToDictionary(c => c.category_id, c => c.name);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy tên loại sản phẩm theo danh sách ID: " + ex.Message, ex);
            }
        }
    }
}
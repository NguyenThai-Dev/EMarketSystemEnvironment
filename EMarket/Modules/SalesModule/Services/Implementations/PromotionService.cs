using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.ProductModule.DTOs;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.SalesModule.DTOs;
using EMarket.Modules.SalesModule.Services.Interfaces;

namespace EMarket.Modules.SalesModule.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IProductCategoryService _productCategoryService;

        public PromotionService(EMarket_DBEntities db, IProductCategoryService productCategoryService)
        {
            _db = db;
            _productCategoryService = productCategoryService;
        }

        public async Task<List<PromotionDTO>> GetAllPromotionsAsync()
        {
            var promos = await _db.Promotions
                .AsNoTracking()
                .ToListAsync();
            var categoryIds = promos
                .Where(p => p.category_id.HasValue)
                .Select(p => p.category_id.Value)
                .Distinct()
                .ToList();
            var categoryDict = await _productCategoryService.GetCategoriesByIdsAsync(categoryIds);
            return promos.Select(p => new PromotionDTO
            {
                PromotionId = p.promotion_id,
                Name = p.name,
                DiscountType = p.discount_type,
                DiscountValue = p.discount_value ?? -1,
                CategoryId = p.category_id,
                Priority = p.priority ?? 0,
                StartDate = p.start_date,
                EndDate = p.end_date,
                IsActive = p.is_active ?? false,
                CategoryName = p.category_id.HasValue && categoryDict.ContainsKey(p.category_id.Value)
                               ? categoryDict[p.category_id.Value]
                               : null,
                CustomerType = p.customer_type
            }).ToList();
        }

        public async Task<List<PromotionDTO>> GetFilteredPromotionAsync(string keyword, int? categoryId, string discountType, string cusType, DateTime? fromDate, DateTime? toDate)
        {
            var query = _db.Promotions
                .Include(p => p.category_id)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string k = keyword.Trim().ToLower();
                query = query.Where(p => p.name.ToLower().Contains(k));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.category_id == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(discountType))
            {
                query = query.Where(p => p.discount_type == discountType);
            }

            if (!string.IsNullOrWhiteSpace(cusType))
            {
                query = query.Where(p => p.customer_type == cusType);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.start_date >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var endOfToDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.end_date <= endOfToDate);
            }

            var rawData = await query
         .OrderByDescending(p => p.is_active)
         .ToListAsync();

            // Lấy danh sách Category Unique từ dữ liệu đã lấy
            var categoryIds = rawData
                .Where(p => p.category_id.HasValue)
                .Select(p => p.category_id.Value)
                .Distinct()
                .ToList();

            var productCateDict = await _productCategoryService.GetCategoriesByIdsAsync(categoryIds);

            return rawData.Select(p => new PromotionDTO
            {
                PromotionId = p.promotion_id,
                Name = p.name,
                DiscountType = p.discount_type,
                DiscountValue = p.discount_value ?? 0,
                StartDate = p.start_date,
                EndDate = p.end_date,
                CategoryName = (p.category_id.HasValue && productCateDict.TryGetValue(p.category_id.Value, out var cate))
                    ? cate : "Không xác định",
                CustomerType = p.customer_type
            }).ToList();
        }

        public async Task<PromotionDTO> GetPromotionByIdAsync(int id)
        {
            var p = await _db.Promotions
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.promotion_id == id);
            if (p == null) return null;
            string categoryName = null;
            if (p.category_id.HasValue)
            {
                var category = await _productCategoryService.GetProductCategoryByIdAsync(p.category_id.Value);
                categoryName = category?.Name;
            }
            return new PromotionDTO
            {
                PromotionId = p.promotion_id,
                Name = p.name,
                DiscountType = p.discount_type,
                DiscountValue = p.discount_value ?? -1,
                CategoryId = p.category_id,
                Priority = p.priority ?? 0,
                StartDate = p.start_date,
                EndDate = p.end_date,
                IsActive = p.is_active ?? false,
                CategoryName = categoryName,
                CustomerType = p.customer_type
            };
        }

        public async Task<int> CreatePromotionAsync(PromotionDTO dto)
        {
            var promo = new Promotion
            {
                name = dto.Name,
                discount_type = dto.DiscountType,
                discount_value = dto.DiscountValue,
                category_id = dto.CategoryId,
                priority = dto.Priority,
                start_date = dto.StartDate,
                end_date = dto.EndDate,
                is_active = dto.IsActive,
                customer_type = dto.CustomerType
            };
            _db.Promotions.Add(promo);
            await _db.SaveChangesAsync();
            return promo.promotion_id;
        }

        public async Task<bool> UpdatePromotionAsync(PromotionDTO dto)
        {
            var promo = await _db.Promotions.FirstOrDefaultAsync(p => p.promotion_id == dto.PromotionId);
            if (promo == null) return false;
            promo.name = dto.Name;
            promo.discount_type = dto.DiscountType;
            promo.discount_value = dto.DiscountValue;
            promo.category_id = dto.CategoryId;
            promo.priority = dto.Priority;
            promo.start_date = dto.StartDate;
            promo.end_date = dto.EndDate;
            promo.is_active = dto.IsActive;
            promo.customer_type = dto.CustomerType;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            var promo = await _db.Promotions.FirstOrDefaultAsync(p => p.promotion_id == id);
            if (promo == null) return false;
            _db.Promotions.Remove(promo);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PromotionDTO>> GetActivePromotionsAsync()
        {
            var now = DateTime.Now;

            return await _db.Promotions
                .AsNoTracking()
                .Where(p => p.is_active == true
                         && p.start_date <= now
                         && p.end_date >= now
                         && p.customer_type == null) // Chỉ lấy KM đại trà theo hàng hóa
                .Select(p => new PromotionDTO
                {
                    PromotionId = p.promotion_id,
                    Name = p.name,
                    DiscountType = p.discount_type,
                    DiscountValue = p.discount_value ?? -1,
                    CategoryId = p.category_id,
                    Priority = p.priority ?? 0,
                    StartDate = p.start_date,
                    EndDate = p.end_date
                })
                .ToListAsync();
        }

        public void ApplyBestPromotion(ProductDTO product, List<PromotionDTO> activePromotions)
        {
            // Set mặc định
            product.OriginalPrice = product.Price ?? 0;
            product.FinalPrice = product.Price ?? 0;
            product.DiscountAmount = 0;
            product.PromotionName = null;

            if (activePromotions == null || !activePromotions.Any()) return;

            // Tìm KM phù hợp nhất:
            // 1. Khớp CategoryId (hoặc KM không set CategoryId - áp dụng toàn bộ)
            // 2. Sắp xếp Priority giảm dần (lấy cái ưu tiên cao nhất)
            var bestPromo = activePromotions
                .Where(p => p.CategoryId == null || p.CategoryId == product.CategoryId)
                .OrderByDescending(p => p.Priority)
                .ThenByDescending(p => p.DiscountValue) // Nếu cùng priority, lấy cái giảm nhiều nhất
                .FirstOrDefault();

            if (bestPromo != null)
            {
                decimal discount = 0;
                if (bestPromo.DiscountType == "Percent")
                {
                    discount = product.OriginalPrice * (bestPromo.DiscountValue / 100);
                }
                else // Amount
                {
                    discount = bestPromo.DiscountValue;
                }

                // Đảm bảo không giảm quá giá gốc (âm tiền)
                if (discount > product.OriginalPrice) discount = product.OriginalPrice;

                product.DiscountAmount = discount;
                product.FinalPrice = product.OriginalPrice - discount;
                product.PromotionName = bestPromo.Name;
            }
        }
    }
}
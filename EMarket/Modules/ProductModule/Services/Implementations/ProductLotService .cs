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
    public class ProductLotService : IProductLotService
    {
        private readonly EMarket_DBEntities _db;

        public ProductLotService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<List<ProductLotDTO>> GetAllProductLotAsync()
        {
            DateTime today = DateTime.Now.Date;
            return await _db.ProductLots
                .Select(l => new ProductLotDTO
                {
                    LotId = l.lot_id,
                    ProductId = l.product_id,
                    ExpiryDate = l.expiry_date,
                    CostPrice = l.cost_price,
                    ManufacturingDate = l.manufacturing_date,
                })
                .Where(l => l.ExpiryDate >= today)
                .ToListAsync();
        }

        public async Task<int?> FindExistingLotIdAsync(int productId, DateTime? manufacturingDate, DateTime? expiryDate)
        {
            // 1. Chuẩn hóa dữ liệu đầu vào về chỉ có Ngày (00:00:00)
            DateTime? mfg = manufacturingDate?.Date;
            DateTime? exp = expiryDate?.Date;

            var query = _db.ProductLots.AsNoTracking().Where(x => x.product_id == productId);

            // 2. Xử lý so sánh Date và NULL một cách an toàn
            if (mfg.HasValue)
                query = query.Where(x => DbFunctions.TruncateTime(x.manufacturing_date) == mfg.Value);
            else
                query = query.Where(x => x.manufacturing_date == null);

            if (exp.HasValue)
                query = query.Where(x => DbFunctions.TruncateTime(x.expiry_date) == exp.Value);
            else
                query = query.Where(x => x.expiry_date == null);

            // 3. Thực thi
            return await query.Select(x => (int?)x.lot_id).FirstOrDefaultAsync();
        }

        public async Task UpdateProductLotCostAsync(ProductLotDTO dto)
        {
            var lot = await _db.ProductLots.FindAsync(dto.LotId);
            if (lot != null)
            {
                lot.cost_price = dto.CostPrice;
            }
        }

        public async Task<List<ProductLotDTO>> GetAllProductLotsByIdsAsync(List<int> ids)
        {
            DateTime today = DateTime.Now.Date;
            return await _db.ProductLots
                .Where(x => ids.Contains(x.product_id))
                .Select(x => new ProductLotDTO
                {
                    LotId = x.lot_id,
                    ProductId = x.product_id,
                    ExpiryDate = x.expiry_date,
                    CostPrice = x.cost_price,
                    ManufacturingDate = x.manufacturing_date,
                    BatchCode = x.batch_code
                })
                .Where(l => l.ExpiryDate >= today)
                .ToListAsync();
        }

        public async Task<ProductLotDTO> GetProductLotByIdAsync(int lotId)
        {
            DateTime today = DateTime.Now.Date;
            var result = await _db.ProductLots
                .Where(l => l.lot_id == lotId)
                .Select(l => new ProductLotDTO
                {
                    LotId = l.lot_id,
                    ProductId = l.product_id,
                    ExpiryDate = l.expiry_date,
                    CostPrice = l.cost_price,
                    ManufacturingDate = l.manufacturing_date,
                })
                .Where(l => l.ExpiryDate >= today)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<List<ProductLotDTO>> GetProductLotsByProductIdAsync(int productId)
        {
            DateTime today = DateTime.Now.Date;
            return await _db.ProductLots
                .AsNoTracking()
                .Where(l => l.product_id == productId)
                .Select(l => new ProductLotDTO
                {
                    LotId = l.lot_id,
                    ProductId = l.product_id,
                    ExpiryDate = l.expiry_date,
                    CostPrice = l.cost_price,
                    ManufacturingDate = l.manufacturing_date,
                    BatchCode = l.batch_code
                })
                .Where(l => l.ExpiryDate >= today)
                .ToListAsync();
        }


        public async Task<List<int>> GetLotIdsByProductAndLotAsync(
    List<int> productIds,
    List<int> lotIds)
        {
            DateTime today = DateTime.Now.Date;
            try
            {
                if (productIds == null || productIds.Count == 0 ||
                    lotIds == null || lotIds.Count == 0)
                    return new List<int>();

                return await _db.ProductLots
                    .AsNoTracking()
                    .Where(l => productIds.Contains(l.product_id)
                             && lotIds.Contains(l.lot_id)
                             && l.expiry_date >= today)
                    .Select(l => l.lot_id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while retrieving lot ids.", ex);
            }
        }


        public async Task<int> CreateProductLotAsync(ProductLotDTO dto)
        {
            try
            {
                var entity = new ProductLot
                {
                    product_id = dto.ProductId,
                    expiry_date = dto.ExpiryDate,
                    cost_price = dto.CostPrice,
                    manufacturing_date = dto.ManufacturingDate,
                };

                _db.ProductLots.Add(entity);
                await _db.SaveChangesAsync();

                return entity.lot_id;
            }
            catch (Exception ex)
            {
                // logging, wrap exceptions if needed
                throw new Exception("Error while creating product lot", ex);
            }
        }

        public async Task<bool> UpdateProductLotAsync(ProductLotDTO dto)
        {
            try
            {
                var entity = await _db.ProductLots.FirstOrDefaultAsync(l => l.lot_id == dto.LotId);
                if (entity == null) return false;

                entity.product_id = dto.ProductId;
                entity.expiry_date = dto.ExpiryDate;
                entity.cost_price = dto.CostPrice;
                entity.manufacturing_date = dto.ManufacturingDate;

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating product lot", ex);
            }
        }

        public async Task<bool> DeleteProductLotAsync(int lotId)
        {
            try
            {
                var entity = await _db.ProductLots.FirstOrDefaultAsync(l => l.lot_id == lotId);
                if (entity == null) return false;

                _db.ProductLots.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deleting product lot", ex);
            }
        }

        public async Task DeleteProductLotsByIdsAsync(List<int> lotIds)
        {
            try
            {
                if (lotIds == null || lotIds.Count == 0)
                    return;

                var lots = await _db.ProductLots
                    .Where(l => lotIds.Contains(l.lot_id))
                    .ToListAsync();

                if (lots.Count == 0)
                    return;

                _db.ProductLots.RemoveRange(lots);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while deleting product lots.", ex);
            }
        }


        public async Task<List<int>> GetLotIdsByProductIdAsync(int productId)
        {
            DateTime today = DateTime.Now.Date;
            return await _db.ProductLots
                .AsNoTracking()
                .Where(l => l.product_id == productId && l.expiry_date >= today)
                .Select(l => l.lot_id)
                .ToListAsync();
        }

        public async Task<List<ProductLotDTO>> GetLotsByIdsAsync(List<int> lotIds)
        {
            DateTime today = DateTime.Now.Date;
            return await _db.ProductLots
                .AsNoTracking()
                .Where(l => lotIds.Contains(l.lot_id) && l.expiry_date >= today)
                .Select(l => new ProductLotDTO
                {
                    LotId = l.lot_id,
                    ProductId = l.product_id,
                    ExpiryDate = l.expiry_date,
                    CostPrice = l.cost_price,
                    ManufacturingDate = l.manufacturing_date,
                    BatchCode = l.batch_code
                })
                .ToListAsync();
        }

    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using ClosedXML.Excel;
using EMarket.Events.Class;
using EMarket.Models;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.DTOs;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.SalesModule.Services.Interfaces;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.ProductModule.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IInventoryService _inventoryService;
        private readonly IBranchService _branchService;
        private readonly IWarehouseService _warehouseService;
        private readonly IProductLotService _productLotService;
        private readonly IPromotionService _promotionService;
        private readonly DateTime defaultDate = new DateTime(2000, 1, 1);

        public ProductService(EMarket_DBEntities db, IInventoryService inventoryService, IBranchService branchService, IWarehouseService warehouseService, IProductLotService productLotService, IPromotionService promotionService)
        {
            _db = db;
            _inventoryService = inventoryService;
            _branchService = branchService;
            _warehouseService = warehouseService;
            _productLotService = productLotService;
            _promotionService = promotionService;
        }

        public async Task<List<ProductDTO>> GetAllProductAsync()
        {
            var products = await _db.Products.AsNoTracking()
                .Select(p => new ProductDTO
                {
                    ProductId = p.product_id,
                    Name = p.name,
                    CategoryId = p.category_id,
                    SupplierId = p.supplier_id,
                    Barcode = p.barcode,
                    Price = p.price,
                    Unit = p.unit,
                    Description = p.description,
                    MinStock = p.min_stock,
                    MaxStock = p.max_stock,
                    Image = p.image
                })
                .ToListAsync();

            return products;
        }

        public async Task<List<object>> GetFilteredProductAsync(
    string keyword,
    int? categoryId,
    int? branchId,
    int? supplierId,
    int? warehouseId)
        {
            try
            {
                // 1. Lấy danh sách sản phẩm khớp filter cơ bản (Name, Barcode, Category, Supplier)
                var query = _db.Products.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var k = keyword.Trim();
                    query = query.Where(p => p.name.Contains(k) || p.barcode.Contains(k));
                }

                if (categoryId.HasValue) query = query.Where(p => p.category_id == categoryId.Value);
                if (supplierId.HasValue) query = query.Where(p => p.supplier_id == supplierId.Value);

                var products = await query
                    .OrderByDescending(p => p.product_id)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.product_id,
                        Name = p.name,
                        Barcode = p.barcode,
                        Price = p.price,
                        Image = p.image,
                        Unit = p.unit,
                        MinStock = p.min_stock,
                        MaxStock = p.max_stock,
                        Description = p.description,
                        CategoryName = p.ProductCategory != null ? p.ProductCategory.name : null,
                        CategoryId = p.ProductCategory != null ? (int?)p.ProductCategory.category_id : null,
                        SupplierName = p.Supplier != null ? p.Supplier.name : null
                    })
                    .ToListAsync();

                if (!products.Any())
                {
                    Debug.WriteLine("No products found matching the filters.");
                    return new List<object>();
                }

                // 2. Lấy dữ liệu bổ trợ (Lot và Inventory) một cách độc lập
                var productIds = products.Select(x => x.ProductId.Value).ToList();

                // Lấy tất cả Lot của các sản phẩm này
                var allLots = await _productLotService.GetAllProductLotsByIdsAsync(productIds) ?? new List<ProductLotDTO>();
                var lotIds = allLots.Select(x => x.LotId).ToList();

                var allInventory = new List<InventoryDTO>();
                if (lotIds.Any())
                {
                    allInventory = await _inventoryService.GetInventoryByProductIdsAsync(productIds, warehouseId, branchId) ?? new List<InventoryDTO>();
                }

                // 3. Tổ chức dữ liệu để Lookup nhanh
                var inventoryLookup = allInventory.GroupBy(x => x.LotId).ToDictionary(g => g.Key, g => g.ToList());
                var lotsLookup = allLots.GroupBy(x => x.ProductId).ToDictionary(g => g.Key, g => g.ToList());

                var result = new List<object>();

                foreach (var p in products)
                {
                    var productLots = lotsLookup.ContainsKey(p.ProductId.Value) ? lotsLookup[p.ProductId.Value] : new List<ProductLotDTO>();
                    var details = new List<object>();
                    decimal totalQuantity = 0;

                    foreach (var lot in productLots)
                    {
                        if (inventoryLookup.TryGetValue(lot.LotId, out var invItems))
                        {
                            foreach (var inv in invItems)
                            {

                                totalQuantity += inv.Quantity;
                                details.Add(new
                                {
                                    inv.WarehouseId,
                                    inv.BranchName,
                                    lot.ExpiryDate,
                                    inv.Quantity,
                                    inv.WarehouseName,
                                    inv.BatchCode
                                });
                            }
                        }
                    }

                    // ĐIỀU KIỆN QUAN TRỌNG: 
                    // Nếu người dùng lọc theo Warehouse mà sản phẩm này không có ở Warehouse đó thì skip
                    if ((warehouseId.HasValue || branchId.HasValue) && !details.Any())
                    {
                        Debug.WriteLine($"Product ID {p.ProductId} skipped due to no inventory in the specified warehouse/branch.");
                        continue;
                    }

                    result.Add(new
                    {
                        p.ProductId,
                        p.Name,
                        p.Barcode,
                        p.CategoryName,
                        p.CategoryId,
                        p.SupplierName,
                        p.Description,
                        p.Price,
                        p.Unit,
                        p.Image,
                        p.MinStock,
                        p.MaxStock,
                        Quantity = totalQuantity,
                        Details = details
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Lỗi khi lấy dữ liệu sản phẩm hỗn hợp.", ex);
            }
        }

        public async Task<List<ProductDTO>> GetFilteredProductAsync(string keyword, int? branchId)
        {
            try
            {
                // =================================================================================
                // STEP 1: Lấy danh sách sản phẩm cơ bản
                // =================================================================================
                var query = _db.Products.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var k = keyword.Trim();
                    query = query.Where(p => p.name.Contains(k) || p.barcode.Contains(k));
                }

                var products = await query
                    .OrderByDescending(p => p.product_id)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.product_id,
                        Name = p.name,
                        Barcode = p.barcode,
                        Price = p.price,
                        // Mặc định
                        OriginalPrice = p.price ?? 0,
                        FinalPrice = p.price ?? 0,
                        Image = p.image,
                        Unit = p.unit,
                        Description = p.description,
                        CategoryId = p.category_id,
                        CategoryName = p.ProductCategory != null ? p.ProductCategory.name : null
                    })
                    .ToListAsync();

                if (!products.Any()) return new List<ProductDTO>();

                // =================================================================================
                // STEP 2: Lấy Lot của các sản phẩm
                // =================================================================================
                var productIds = products.Select(x => x.ProductId.Value).ToList();

                var allLots = await _productLotService.GetAllProductLotsByIdsAsync(productIds) ?? new List<ProductLotDTO>();

                // Nếu không có Lot nào thì trả về luôn (vì không có hàng để bán)
                if (!allLots.Any()) return products;

                var lotIds = allLots.Select(x => x.LotId).ToList();

                // =================================================================================
                // STEP 3: Lấy Inventory và Filter theo Branch (SỬA LẠI CHỖ NÀY)
                // =================================================================================

                // 3.1 Lấy tất cả tồn kho của các Lot này (Chưa lọc Branch)
                // Truyền null cho warehouseId vì ta chưa biết warehouse nào, ta cần lọc sau khi lấy về
                var allInventory = await _inventoryService.GetInventoryByProductIdsAsync(productIds, null) ?? new List<InventoryDTO>();

                // 3.2 Lọc Inventory theo BranchId (Logic: Inventory -> Warehouse -> Branch)
                if (branchId.HasValue && allInventory.Any())
                {
                    // Lấy danh sách ID các kho thuộc Branch hiện tại
                    var validWarehouseIds = await _db.Warehouses
                        .AsNoTracking()
                        .Where(w => w.branch_id == branchId.Value)
                        .Select(w => w.warehouse_id)
                        .ToListAsync();

                    // Chỉ giữ lại các Inventory nằm trong các kho này
                    allInventory = allInventory
                        .Where(inv => validWarehouseIds.Contains(inv.WarehouseId))
                        .ToList();
                }

                // =================================================================================
                // STEP 4: Build Lookup & Tính tổng
                // =================================================================================
                var inventoryLookup = allInventory.GroupBy(x => x.LotId).ToDictionary(g => g.Key, g => g.ToList());
                var lotsLookup = allLots.GroupBy(x => x.ProductId).ToDictionary(g => g.Key, g => g.ToList());

                foreach (var p in products)
                {
                    decimal totalQuantity = 0;

                    if (p.ProductId.HasValue && lotsLookup.TryGetValue(p.ProductId.Value, out var productLots))
                    {
                        foreach (var lot in productLots)
                        {
                            // Check xem Lot này có tồn kho (đã được lọc theo Branch ở trên) không
                            if (inventoryLookup.TryGetValue(lot.LotId, out var invItems))
                            {
                                totalQuantity += invItems.Sum(x => x.Quantity);
                            }
                        }
                    }

                    p.Quantity = (int?)totalQuantity;
                }

                // =================================================================================
                // STEP 5: Áp dụng Khuyến Mãi
                // =================================================================================
                var activePromos = await _promotionService.GetActivePromotionsAsync();

                foreach (var p in products)
                {
                    _promotionService.ApplyBestPromotion(p, activePromos);
                }

                return products;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Lỗi khi lấy dữ liệu sản phẩm POS.", ex);
            }
        }

        public async Task<List<ProductDTO>> GetProductsByIdsAsync(List<int> ids)
        {
            return await _db.Products
                .Where(x => ids.Contains(x.product_id))
                .Select(x => new ProductDTO
                {
                    ProductId = x.product_id,
                    Name = x.name,
                    CategoryId = x.category_id,
                    SupplierId = x.supplier_id,
                    Barcode = x.barcode,
                    Price = x.price,
                    Unit = x.unit,
                    Description = x.description,
                    MinStock = x.min_stock,
                    MaxStock = x.max_stock,
                    Image = x.image
                })
                .ToListAsync();
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            try
            {
                // 1) Lấy product
                var productEntity = await _db.Products
                    .FirstOrDefaultAsync(x => x.product_id == id);

                if (productEntity == null)
                    return null;

                // 2) Lấy category + supplier
                var categoryName = await _db.ProductCategories
                    .Where(c => c.category_id == productEntity.category_id)
                    .Select(c => c.name)
                    .FirstOrDefaultAsync();

                var supplierName = await _db.Suppliers
                    .Where(s => s.supplier_id == productEntity.supplier_id)
                    .Select(s => s.name)
                    .FirstOrDefaultAsync();

                // 3) Lấy tất cả lot of product
                var lotItems = await _db.ProductLots
                    .Where(pl => pl.product_id == id)
                    .ToListAsync();

                // 4) Gom lot_ids
                var lotIds = lotItems.Select(li => li.lot_id).ToList();

                // 5) Lấy inventory FOR ALL LOTS (1 query duy nhất)
                var inventories = await _db.Inventories
                    .Where(inv => lotIds.Contains(inv.lot_id))
                    .ToListAsync();

                var totalQuantity = inventories.Sum(i => i.quantity ?? 0);

                // 6) Map DTO
                var dto = new ProductDTO
                {
                    ProductId = productEntity.product_id,
                    Name = productEntity.name,
                    CategoryId = productEntity.category_id,
                    SupplierId = productEntity.supplier_id,
                    Barcode = productEntity.barcode,
                    Price = productEntity.price,
                    Unit = productEntity.unit,
                    Description = productEntity.description,
                    MinStock = productEntity.min_stock,
                    MaxStock = productEntity.max_stock,
                    Image = productEntity.image,

                    CategoryName = categoryName,
                    SupplierName = supplierName,

                    ExpiredAt = lotItems.Any() ? lotItems.Min(li => li.expiry_date) : defaultDate,

                    Quantity = totalQuantity
                };

                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving product", ex);
            }
        }

        public async Task<int> CreateProductAsync(ProductDTO dto, HttpPostedFileBase file)
        {
            try
            {
                // 1. Tạo Entity ban đầu
                var entity = new Product
                {
                    name = dto.Name,
                    category_id = dto.CategoryId,
                    supplier_id = dto.SupplierId,
                    barcode = dto.Barcode,
                    price = dto.Price,
                    unit = dto.Unit,
                    min_stock = dto.MinStock,
                    max_stock = dto.MaxStock,
                    description = dto.Description,
                    created_at = DateTime.Now
                };

                _db.Products.Add(entity);
                await _db.SaveChangesAsync(); // Lấy được product_id

                // ===============================
                // XỬ LÝ ẢNH (Logic mới)
                // ===============================
                string productFolder = HttpContext.Current.Server.MapPath($"~/Uploads/Products/{entity.product_id}");
                Directory.CreateDirectory(productFolder);

                string finalImageDbPath = null; // Biến lưu đường dẫn ảnh cuối cùng để update DB
                bool imageHandled = false;      // Cờ đánh dấu đã xử lý xong ảnh chưa

                // ===== CÁCH 1: KIỂM TRA ẢNH TỪ TEMP (dto.Image) =====
                if (!string.IsNullOrWhiteSpace(dto.Image))
                {
                    string tempPath = HttpContext.Current.Server.MapPath($"~/Temp/Products/{dto.Image}");

                    if (File.Exists(tempPath))
                    {
                        // Logic di chuyển file
                        string destPath = Path.Combine(productFolder, dto.Image);

                        // Xử lý trùng tên
                        if (File.Exists(destPath))
                        {
                            string ext = Path.GetExtension(dto.Image);
                            string newName = Guid.NewGuid() + ext;
                            destPath = Path.Combine(productFolder, newName);
                            dto.Image = newName; // Cập nhật lại tên mới
                        }

                        File.Move(tempPath, destPath);

                        // Ghi nhận đường dẫn
                        finalImageDbPath = $"/Uploads/Products/{entity.product_id}/{dto.Image}";
                        imageHandled = true; // Đánh dấu là đã có ảnh
                    }
                }

                // ===== CÁCH 2: FALLBACK - FILE UPLOAD TAY =====
                // Chỉ chạy nếu Cách 1 chưa xử lý được (imageHandled == false)
                if (!imageHandled && file != null && file.ContentLength > 0)
                {
                    string ext = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid() + ext;
                    string destPath = Path.Combine(productFolder, fileName);

                    file.SaveAs(destPath);

                    finalImageDbPath = $"/Uploads/Products/{entity.product_id}/{fileName}";
                    imageHandled = true;
                }

                // ===== CẬP NHẬT DB MỘT LẦN CUỐI =====
                if (imageHandled && finalImageDbPath != null)
                {
                    entity.image = finalImageDbPath;
                    await _db.SaveChangesAsync();
                }

                return entity.product_id;
            }
            catch (Exception ex)
            {
                throw new Exception("CreateProduct failed: " + ex.Message, ex);
            }
        }

        public async Task<bool> UpdateProductAsync(ProductDTO dto, string rootPath)
        {
            // 1. Lấy Entity cũ từ Database
            var entity = await _db.Products.FirstOrDefaultAsync(p => p.product_id == dto.ProductId);
            if (entity == null) return false;

            // 2. Cập nhật các trường thông thường
            entity.name = dto.Name;
            entity.barcode = dto.Barcode;
            entity.unit = dto.Unit;
            entity.description = dto.Description;
            entity.category_id = dto.CategoryId;
            entity.supplier_id = dto.SupplierId;
            entity.price = dto.Price;
            entity.min_stock = dto.MinStock;
            entity.max_stock = dto.MaxStock;
            entity.updated_at = DateTime.Now;

            if (!string.IsNullOrEmpty(dto.Image))
            {
                // 1. Logic Xóa ảnh cũ
                if (!string.IsNullOrEmpty(entity.image))
                {
                    string relativeFilePath = entity.image.TrimStart('~').TrimStart('/');
                    string pathToDelete = System.IO.Path.Combine(rootPath, relativeFilePath);

                    if (System.IO.File.Exists(pathToDelete))
                    {
                        try { System.IO.File.Delete(pathToDelete); }
                        catch { /* Log if needed */ }
                    }
                }

                // 2. Cập nhật path ảnh mới
                entity.image = dto.Image;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var entity = await _db.Products.FindAsync(id);
                    if (entity == null) return false;

                    _db.Products.Remove(entity);
                    await _db.SaveChangesAsync();

                    scope.Complete();
                }
                catch
                {
                    return false;
                }
            }

            // ===== SAU KHI DB OK → XÓA FILE =====
            try
            {
                string productFolder = HttpContext.Current.Server.MapPath($"~/Uploads/Products/{id}");

                if (Directory.Exists(productFolder))
                {
                    Directory.Delete(productFolder, true); // recursive
                }
            }
            catch
            {
                // log là đủ, KHÔNG throw
            }

            return true;
        }


        public async Task<List<ProductImageDTO>> GetAllProductImageByProductIdAsync(int productId)
        {
            return await _db.ProductImages
                .Where(x => x.product_id == productId)
                .Select(x => new ProductImageDTO
                {
                    ImageId = x.image_id,
                    ProductId = x.product_id,
                    ImageUrl = x.image_url,
                    SortOrder = x.sort_order ?? 1,
                    CreatedAt = x.created_at ?? defaultDate
                })
                .ToListAsync();
        }

        public async Task<ProductImageDTO> GetProductImageByIdAsync(int id)
        {
            var x = await _db.ProductImages.FirstOrDefaultAsync(p => p.image_id == id);

            if (x == null) return null;

            return new ProductImageDTO
            {
                ImageId = x.image_id,
                ProductId = x.product_id,
                ImageUrl = x.image_url,
                SortOrder = x.sort_order ?? 1,
                CreatedAt = x.created_at ?? DateTime.Now.AddYears(-100)
            };
        }

        public async Task<ProductImageDTO> CreateProductImageAsync(ProductImageDTO dto)
        {
            try
            {
                var entity = new ProductImage
                {
                    product_id = dto.ProductId,
                    image_url = dto.ImageUrl,
                    sort_order = dto.SortOrder,
                    created_at = DateTime.Now
                };

                _db.ProductImages.Add(entity);
                await _db.SaveChangesAsync();

                dto.ImageId = entity.image_id;
                dto.CreatedAt = entity.created_at ?? DateTime.Now;

                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating ProductImage: " + ex.Message);
            }
        }

        public async Task<bool> DeleteProductImageAsync(int id)
        {
            try
            {
                var entity = await _db.ProductImages.FindAsync(id);
                if (entity == null) return false;

                _db.ProductImages.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting ProductImage: " + ex.Message);
            }
        }

        private readonly string _tempRoot = "~/Temp/Products/";
        private readonly string _productRoot = "~/Uploads/Products/";

        public TempImageDTO UploadTempImage(HttpPostedFileBase file)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var absoluteTemp = HttpContext.Current.Server.MapPath(_tempRoot + fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(absoluteTemp));
            file.SaveAs(absoluteTemp);

            return new TempImageDTO
            {
                FileName = fileName,
                TempPath = "/Temp/Products/" + fileName
            };
        }

        public async Task<List<ProductImageDTO>> MoveTempImagesToProductAsync(int productId, List<string> tempFiles)
        {
            var result = new List<ProductImageDTO>();

            if (tempFiles == null || tempFiles.Count == 0)
                return result;

            // Tạo folder Uploads/Products/[productId]/
            string destFolder = HttpContext.Current.Server.MapPath(_productRoot + productId + "/");

            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            foreach (var rawName in tempFiles)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(rawName))
                        continue;

                    // Chuẩn hóa tên file — tránh Path Traversal
                    string fileName = Path.GetFileName(rawName);

                    // Source path
                    string source = HttpContext.Current.Server.MapPath(_tempRoot + fileName);

                    if (!File.Exists(source))
                        continue;

                    // Destination path
                    string dest = Path.Combine(destFolder, fileName);

                    // Nếu file tồn tại -> tránh ghi đè bằng tên mới
                    if (File.Exists(dest))
                    {
                        string nameOnly = Path.GetFileNameWithoutExtension(fileName);
                        string ext = Path.GetExtension(fileName);
                        string newName = $"{nameOnly}_{Guid.NewGuid():N}{ext}";
                        dest = Path.Combine(destFolder, newName);
                        fileName = newName;
                    }

                    // Move file
                    File.Move(source, dest);

                    // Image URL trả về client
                    string imageUrl = $"{_productRoot.TrimStart('~')}{productId}/{fileName}";

                    // Tạo DTO để insert DB
                    var dto = new ProductImageDTO
                    {
                        ProductId = productId,
                        ImageUrl = imageUrl,
                        SortOrder = 0
                    };

                    // Insert vào DB
                    var inserted = await CreateProductImageAsync(dto);

                    // Lưu vào kết quả trả ra client
                    result.Add(inserted);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return result;
        }

        public bool DeleteTempImageAsync(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return false;

                string tempFolder = HttpContext.Current.Server.MapPath("/Temp/Products/");
                string filePath = Path.Combine(tempFolder, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public byte[] GenerateProductImportTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Products");

                // ================= HEADER =================
                string[] headers =
                {
            "Name",
            "Category",
            "Supplier",
            "Barcode",
            "Price",
            "Unit",
            "MinStock",
            "MaxStock",
            "Quantity",
            "ThumbnailUrl",
            "ImageUrls",
            "Description"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cell(1, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                // ================= COLUMN WIDTH =================
                ws.Columns().AdjustToContents();
                ws.Column(4).Style.NumberFormat.Format = "@";
                ws.Rows().AdjustToContents();

                // ================= SAMPLE DATA =================
                ws.Cell(2, 1).Value = "Coca Cola 330ml";
                ws.Cell(2, 2).Value = 1;
                ws.Cell(2, 3).Value = 1;
                ws.Cell(2, 4).Value = "8934567890123";
                ws.Cell(2, 5).Value = 10000;
                ws.Cell(2, 6).Value = "Chai";
                ws.Cell(2, 7).Value = 10;
                ws.Cell(2, 8).Value = 500;
                ws.Cell(2, 9).Value = 100;
                ws.Cell(2, 10).Value = "https://cdn.example.com/products/coca-thumb.jpg";
                ws.Cell(2, 11).Value =
                    "https://cdn.example.com/products/coca-1.jpg|https://cdn.example.com/products/coca-2.jpg";
                ws.Cell(2, 12).Value = "Nước ngọt có gas";

                // ================= REQUIRED VALIDATION =================
                AddRequired(ws, "A2:A1000"); // Name
                AddRequired(ws, "B2:B1000"); // Category
                AddRequired(ws, "C2:C1000"); // Supplier
                AddRequired(ws, "D2:D1000"); // Barcode
                AddRequired(ws, "E2:F1000"); // Price
                AddRequired(ws, "F2:I1000"); // Unit

                // ================= NUMBER VALIDATION =================
                AddNumber(ws, "B2:C1000");
                AddNumber(ws, "E2:E1000");
                AddNumber(ws, "G2:I1000");

                // ================= URL HINT (OPTIONAL) =================
                ws.Range("K2:K1000").CreateDataValidation().AllowedValues = XLAllowedValues.AnyValue;
                ws.Range("L2:L1000").CreateDataValidation().AllowedValues = XLAllowedValues.AnyValue;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private void AddRequired(IXLWorksheet ws, string range)
        {
            var col = new string(range.TakeWhile(char.IsLetter).ToArray());

            var dv = ws.Range(range).CreateDataValidation();

            // CÁCH MỚI: Truyền công thức trực tiếp vào phương thức Custom
            dv.Custom($"=LEN(TRIM({col}2))>0");

            dv.ShowErrorMessage = true;
            dv.ErrorTitle = "Thiếu dữ liệu";
            dv.ErrorMessage = "Trường này là bắt buộc";
        }

        private void AddNumber(IXLWorksheet ws, string range)
        {
            var dv = ws.Range(range).CreateDataValidation();

            // SỬA LẠI: Sử dụng EqualOrGreaterThan thay vì GreaterThanOrEqualTo
            dv.Decimal.EqualOrGreaterThan(0);

            dv.ShowErrorMessage = true;
            dv.ErrorTitle = "Sai dữ liệu";
            dv.ErrorMessage = "Giá trị phải là số >= 0";
        }

        private string NormalizeUrl(string rawUrl)
        {
            if (string.IsNullOrWhiteSpace(rawUrl))
                return null;

            rawUrl = rawUrl.Trim();

            if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out var uri))
                throw new Exception("Invalid image URL format");

            var builder = new UriBuilder(uri)
            {
                Path = Uri.EscapeUriString(uri.LocalPath)
            };

            return builder.Uri.AbsoluteUri;
        }


        private async Task<string> DownloadToTempAsync(string rawUrl)
        {
            try
            {
                var safeUrl = NormalizeUrl(rawUrl);

                using (var client = new HttpClient())
                {
                    var bytes = await client.GetByteArrayAsync(safeUrl);

                    var ext = Path.GetExtension(new Uri(safeUrl).AbsolutePath);
                    var fileName = Guid.NewGuid() + ext;

                    var tempPath = HttpContext.Current.Server.MapPath("~/Temp/Products/" + fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(tempPath));

                    File.WriteAllBytes(tempPath, bytes);

                    return fileName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot download image: {rawUrl}. {ex.Message}");
            }
        }

        public async Task<Dictionary<int, string>> GetProductNamesByIdsAsync(List<int> productIds)
        {
            return await _db.Products
                .Where(p => productIds.Contains(p.product_id))
                .ToDictionaryAsync(p => p.product_id, p => p.name);
        }


        public async Task<ProductImportResult> ImportAsync(HttpPostedFileBase excelFile, string rootPath)
        {
            var result = new ProductImportResult
            {
                Success = false,
                ImportedRows = 0,
                TotalRows = 0
            };

            var errorRows = new List<(int Row, string Error)>();

            using (var workbook = new XLWorkbook(excelFile.InputStream))
            {
                var ws = workbook.Worksheet("Products");
                var lastRow = ws.LastRowUsed().RowNumber();
                result.TotalRows = lastRow - 1;

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        for (int row = 2; row <= lastRow; row++)
                        {
                            try
                            {
                                // 1. Build DTO
                                var dto = new ProductDTO
                                {
                                    Name = ws.Cell(row, 1).GetString(),
                                    CategoryId = ws.Cell(row, 2).GetValue<int>(),
                                    SupplierId = ws.Cell(row, 3).GetValue<int>(),
                                    Barcode = ws.Cell(row, 4).GetString(),
                                    Price = ws.Cell(row, 5).GetValue<decimal>(),
                                    Unit = ws.Cell(row, 6).GetString(),
                                    MinStock = ws.Cell(row, 7).GetValue<int>(),
                                    MaxStock = ws.Cell(row, 8).GetValue<int>(),
                                    Quantity = ws.Cell(row, 9).GetValue<int>(),
                                    Description = ws.Cell(row, 12).GetString()
                                };

                                // ============================
                                // THUMBNAIL (KHÔNG ĐƯA VÀO ProductImages)
                                // ============================
                                var thumbnailUrl = ws.Cell(row, 10).GetString();
                                if (!string.IsNullOrWhiteSpace(thumbnailUrl))
                                {
                                    dto.Image = await DownloadToTempAsync(thumbnailUrl);
                                }

                                // 2. Create Product (service tự move thumbnail vào đúng folder)
                                var productId = await CreateProductAsync(dto, null);

                                // ============================
                                // PRODUCT IMAGES (ĐÚNG CHỖ)
                                // ============================
                                var tempFiles = new List<string>();

                                var imageUrls = ws.Cell(row, 11).GetString()
                                    .Split('|')
                                    .Select(x => x.Trim())
                                    .Where(x => !string.IsNullOrWhiteSpace(x));

                                foreach (var url in imageUrls)
                                {
                                    var tempImg = await DownloadToTempAsync(url);
                                    if (!string.IsNullOrEmpty(tempImg))
                                        tempFiles.Add(tempImg);
                                }

                                if (tempFiles.Any())
                                {
                                    await MoveTempImagesToProductAsync(productId, tempFiles);
                                }


                                result.ImportedRows++;
                            }
                            catch (Exception exRow)
                            {
                                errorRows.Add((row, exRow.Message));
                            }
                        }
                        if (errorRows.Any())
                        {
                            var errorFile = BuildErrorReport(errorRows);
                            result.ErrorToken = SaveErrorReport(errorFile);
                            return result;
                        }

                        scope.Complete();
                        result.Success = true;
                    }
                    catch
                    {
                        // rollback by TransactionScope
                    }
                }
            }

            return result;
        }


        private byte[] BuildErrorReport(List<(int Row, string Error)> errors)
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet("Errors");
                ws.Cell(1, 1).Value = "Row";
                ws.Cell(1, 2).Value = "Error";

                int r = 2;
                foreach (var e in errors)
                {
                    ws.Cell(r, 1).Value = e.Row;
                    ws.Cell(r, 2).Value = e.Error;
                    r++;
                }

                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }

        private static readonly ObjectCache _cache = MemoryCache.Default;

        private string SaveErrorReport(byte[] content)
        {
            var token = Guid.NewGuid().ToString("N");

            _cache.Set(
                "IMPORT_ERR_" + token,
                content,
                DateTimeOffset.Now.AddMinutes(30)
            );

            return token;
        }

        public byte[] GetErrorReport(string token)
        {
            return _cache.Get("IMPORT_ERR_" + token) as byte[];
        }


        public async Task<List<LowStockAlertDTO>> ReadLowStockAlertsAsync(int top = 10)
        {
            try
            {
                /*
                 * Tư duy mới (chuẩn module):
                 * - Inventory chỉ dùng để SUM quantity theo (Product + Warehouse)
                 * - Product / Category join trực tiếp (cùng module)
                 * - Warehouse / Branch: LẤY TÊN QUA SERVICE
                 */

                var rawData = await (
                    from i in _db.Inventories.AsNoTracking()

                    join pl in _db.ProductLots on i.lot_id equals pl.lot_id
                    join p in _db.Products on pl.product_id equals p.product_id
                    join c in _db.ProductCategories on p.category_id equals c.category_id

                    group i by new
                    {
                        p.product_id,
                        p.name,
                        p.min_stock,
                        CategoryName = c.name,
                        WarehouseId = i.warehouse_id
                    }
                    into g

                    let currentStock = g.Sum(x => x.quantity ?? 0)

                    where g.Key.min_stock.HasValue
                       && currentStock < g.Key.min_stock.Value

                    select new
                    {
                        g.Key.product_id,
                        g.Key.name,
                        g.Key.min_stock,
                        g.Key.CategoryName,
                        g.Key.WarehouseId,
                        CurrentStock = currentStock
                    }
                ).ToListAsync();

                // ===== LẤY DỮ LIỆU NGOÀI MODULE =====
                var warehouseDict = await _warehouseService.GetWarehouseDictAsync();
                var branchDict = await _branchService.GetBranchDictAsync();

                var result = rawData.Select(x =>
                {
                    warehouseDict.TryGetValue(x.WarehouseId, out var wh);
                    BranchDTO branch = null;

                    if (wh != null)
                        branchDict.TryGetValue(wh.BranchId, out branch);

                    return new LowStockAlertDTO
                    {
                        ProductId = x.product_id,
                        ProductName = x.name,
                        CategoryName = x.CategoryName,

                        CurrentStock = x.CurrentStock,
                        MinStock = x.min_stock ?? 0,

                        WarehouseId = x.WarehouseId,
                        WarehouseName = wh?.Name ?? "N/A",

                        BranchId = wh?.BranchId ?? 0,
                        BranchName = branch?.Name ?? "N/A"
                    };
                })
                .ToList();

                /*
                 * SORT NGHIỆP VỤ:
                 * 1. Nguy cấp: <= 50% MinStock
                 * 2. Sau đó sort theo tỷ lệ tồn kho
                 */
                return result
                    .OrderBy(x => x.CurrentStock <= x.MinStock * 0.5m ? 0 : 1)
                    .ThenBy(x => x.MinStock == 0 ? 1 : (decimal)x.CurrentStock / x.MinStock)
                    .Take(top)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load low stock alerts.", ex);
            }
        }

    }
}
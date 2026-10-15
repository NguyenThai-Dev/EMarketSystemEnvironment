# API Manual: Products Catalog, Categories, Suppliers, Lots & Barcode

> **Module Base Path:** `/api/admin/product`  
> **Header Yêu Cầu:** `Authorization: Bearer <token>`, `Content-Type: application/json`

---

## 1. Products (Quản lý Sản Phẩm)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/product/products` | Không | Lấy toàn bộ danh sách sản phẩm |
| `GET` | `/api/admin/product/products/paged` | `start=0&length=10&keyword=&categoryId=&branchId=` | Phân trang danh sách sản phẩm |
| `GET` | `/api/admin/product/products/search` | `keyword=&categoryId=&branchId=&supplierId=&warehouseId=` | Tìm kiếm sản phẩm theo đa tiêu chí |
| `GET` | `/api/admin/product/products/search-simple` | `keyword=&branchId=` | Tìm kiếm nhanh sản phẩm (cho autocomplete / search bar) |
| `GET` | `/api/admin/product/products/barcode/{barcode}`<br>`/api/admin/product/products/by-barcode` | Path / Query: `barcode`, `branchId` | **Quét mã Barcode**: Tra cứu thông tin sản phẩm và tồn kho ngay lập tức |
| `GET` | `/api/admin/product/products/{id}` | Path: `id` | Lấy chi tiết thông tin một sản phẩm |
| `POST` | `/api/admin/product/products/by-ids` | Body: `[1, 2, 3]` | Lấy danh sách sản phẩm theo danh sách ID |
| `POST` | `/api/admin/product/products/names-by-ids` | Body: `[1, 2, 3]` | Lấy map ID -> Tên sản phẩm |
| `POST` | `/api/admin/product/products` | `ProductDTO` | Tạo mới sản phẩm |
| `PUT` / `POST` | `/api/admin/product/products`<br>`/api/admin/product/products/update` | `ProductDTO` | Cập nhật thông tin sản phẩm |
| `DELETE` / `POST` | `/api/admin/product/products/{id}`<br>`/api/admin/product/products/delete/{id}` | Path: `id` | Xóa hoặc vô hiệu hóa sản phẩm |
| `POST` | `/api/admin/product/products/active` | Body: `{ "ProductId": 1, "MinStock": 10, "MaxStock": 100 }` | Kích hoạt lại sản phẩm đã bị ẩn/khóa |
| `GET` | `/api/admin/product/products/inactive` | `keyword=&categoryId=&branchId=&supplierId=&warehouseId=` | Danh sách sản phẩm đang bị tạm ngưng kinh doanh |
| `GET` | `/api/admin/product/products/low-stock-alerts` | `top=10` | Danh sách cảnh báo sản phẩm chạm ngưỡng tồn kho tối thiểu |

### Cấu trúc DTO Sản Phẩm (`ProductDTO`)
```json
{
  "ProductId": 10,
  "Name": "Sữa Tươi Tiệt Trùng Vinamilk 1L",
  "Barcode": "8934673123456",
  "CategoryId": 2,
  "CategoryName": "Sữa & Chế Phẩm Sữa",
  "SupplierId": 3,
  "SupplierName": "Công ty Cổ phần Vinamilk",
  "CostPrice": 28000,
  "SalePrice": 35000,
  "Unit": "Hộp",
  "MinStock": 10,
  "MaxStock": 200,
  "IsActive": true,
  "ImageUrl": "/Uploads/Products/vinamilk-1l.jpg",
  "TotalStock": 85
}
```

---

## 2. Product Images (Quản lý Hình Ảnh Sản Phẩm)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/product/products/{productId}/images` | Path: `productId` | Lấy danh sách ảnh phụ của sản phẩm |
| `POST` | `/api/admin/product/products/images` | `ProductImageDTO` | Thêm URL ảnh mới cho sản phẩm |
| `DELETE` / `POST` | `/api/admin/product/products/images/{id}`<br>`/api/admin/product/products/images/delete/{id}` | Path: `id` | Xóa hình ảnh |

```json
// ProductImageDTO
{
  "ImageId": 1,
  "ProductId": 10,
  "ImageUrl": "/Uploads/Products/vnm_detail_1.jpg",
  "IsMain": false
}
```

---

## 3. Product Categories (Danh Mục Ngành Hàng)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/product/categories` | Không | Lấy toàn bộ danh mục sản phẩm |
| `GET` | `/api/admin/product/categories/{id}` | Path: `id` | Lấy thông tin một danh mục |
| `POST` | `/api/admin/product/categories` | `ProductCategoryDTO` | Tạo danh mục mới |
| `PUT` / `POST` | `/api/admin/product/categories`<br>`/api/admin/product/categories/update` | `ProductCategoryDTO` | Cập nhật danh mục |
| `DELETE` / `POST` | `/api/admin/product/categories/{id}`<br>`/api/admin/product/categories/delete/{id}` | Path: `id` | Xóa danh mục |

```json
// ProductCategoryDTO
{
  "CategoryId": 2,
  "Name": "Sữa & Chế Phẩm Sữa",
  "Description": "Các loại sữa chua, sữa hạt, sữa tươi",
  "IsActive": true
}
```

---

## 4. Suppliers (Nhà Cung Cấp)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/product/suppliers` | Không | Lấy toàn bộ nhà cung cấp |
| `GET` | `/api/admin/product/suppliers/{id}` | Path: `id` | Lấy chi tiết một nhà cung cấp |
| `POST` | `/api/admin/product/suppliers` | `SupplierDTO` | Tạo mới nhà cung cấp |
| `PUT` / `POST` | `/api/admin/product/suppliers`<br>`/api/admin/product/suppliers/update` | `SupplierDTO` | Cập nhật thông tin nhà cung cấp |
| `DELETE` / `POST` | `/api/admin/product/suppliers/{id}`<br>`/api/admin/product/suppliers/delete/{id}` | Path: `id` | Xóa nhà cung cấp |

```json
// SupplierDTO
{
  "SupplierId": 3,
  "Name": "Công ty Cổ phần Vinamilk",
  "ContactName": "Anh Hoàng - Quản lý Phân phối",
  "Phone": "02854155555",
  "Email": "vinamilk@vinamilk.com.vn",
  "Address": "10 Tân Trào, P. Tân Phú, Quận 7, TP.HCM",
  "IsActive": true
}
```

---

## 5. Product Lots / Batches (Quản lý Lô Hàng & Hạn Dùng FEFO)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/product/lots` | Không | Lấy toàn bộ các lô hàng trong hệ thống |
| `GET` | `/api/admin/product/lots/{id}` | Path: `id` | Lấy chi tiết lô hàng |
| `GET` | `/api/admin/product/lots/by-product/{productId}` | Path: `productId` | Lấy danh sách các lô hàng của một sản phẩm (để chọn khi bán / xuất kho) |
| `POST` | `/api/admin/product/lots` | `ProductLotDTO` | Tạo lô hàng mới |
| `PUT` / `POST` | `/api/admin/product/lots`<br>`/api/admin/product/lots/update` | `ProductLotDTO` | Cập nhật thông tin lô hàng |
| `DELETE` / `POST` | `/api/admin/product/lots/{id}`<br>`/api/admin/product/lots/delete/{id}` | Path: `id` | Xóa lô hàng |

```json
// ProductLotDTO
{
  "LotId": 5,
  "LotNumber": "LOT-202610-01",
  "ProductId": 10,
  "ManufactureDate": "2026-10-01T00:00:00",
  "ExpiryDate": "2027-04-01T00:00:00",
  "Quantity": 150,
  "WarehouseId": 1
}
```

---

## Hướng Dẫn Tích Hợp Flutter / Android

1. **Camera Barcode Scanning:** Tích hợp package `mobile_scanner`. Khi camera nhận diện mã vạch, gọi ngay endpoint `GET /api/admin/product/products/barcode/{barcode}`.
   - Nếu tìm thấy: hiển thị Pop-up chi tiết hoặc tự động thêm vào giỏ hàng POS với số lượng +1.
   - Nếu không tìm thấy: hiển thị thông báo "Sản phẩm chưa có trong hệ thống, bạn có muốn tạo mới không?".
2. **Offline Caching:** Cache danh mục (`categories`) và nhà cung cấp (`suppliers`) trong bộ nhớ máy bằng `Isar` hoặc `Hive` để giao diện mượt mà và giảm tải request.

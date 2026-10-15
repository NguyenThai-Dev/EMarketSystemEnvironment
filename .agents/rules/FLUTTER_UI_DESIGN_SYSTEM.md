# FLUTTER UI/UX DESIGN SYSTEM & ENGINEERING RULES
### Quy Chuẩn Thiết Kế Giao Diện Đồng Bộ, Cao Cấp & Trải Nghiệm Chuẩn Native Cho EMarket Mobile

---

## 1. TRIẾT LÝ THIẾT KẾ (DESIGN PHILOSOPHY)
* **Phong cách tổng thể (Visual Style)**: Hiện đại, sạch sẽ, tối giản và chuyên nghiệp dành cho ứng dụng quản lý bán lẻ & ERP di động (tương tự trải nghiệm của Square POS, Shopify POS).
* **Nền tảng màu sắc (Color Foundation)**: Sử dụng nền Slate nhạt (`#F8FAFC`) kết hợp các khối thẻ màu trắng tinh khôi (`#FFFFFF`), đường viền siêu mảnh (`#E2E8F0`) để tạo chiều sâu thay vì dùng shadow đổ bóng đen đậm thô ráp.
* **Tối ưu thao tác một tay (Thumb-Zone Ergonomics)**:
  - Tất cả các nút bấm hành động then chốt (Thanh toán, Quét mã Barcode, Thêm vào giỏ hàng, Lưu phiếu) phải đặt ở nửa dưới màn hình hoặc ghim cố định ở thanh Bottom Action Bar.
  - Hạn chế tối đa việc bắt người dùng với tay lên góc trên màn hình để thực hiện hành động chính.
* **Không dùng dữ liệu giả dạng xấu**: Luôn hiển thị dữ liệu tài chính với định dạng chuẩn VNĐ (`150.000 ₫`), ngày giờ rõ ràng, có avatar/placeholder bo góc mềm mại.

---

## 2. BẢNG MÃ MÀU CHUẨN HÓA (SEMANTIC COLOR TOKENS)

Mọi widget và giao diện UI bắt buộc sử dụng màu thông qua `AppColors` tại `lib/core/constants/app_colors.dart`:

| Token | Mã Hex | Ý nghĩa sử dụng |
| :--- | :--- | :--- |
| `AppColors.primary` | `#1B365D` | Deep Navy: Màu thương hiệu chính, AppBar, nút bấm chính, thẻ Hero Dashboard |
| `AppColors.primaryLight` | `#2E75B6` | Modern Blue: Điểm nhấn, thanh tiến trình, icon nổi bật, text giá tiền |
| `AppColors.secondary` | `#00A896` | Emerald Teal: Hành động phụ, nhãn thành công, trạng thái tích cực |
| `AppColors.background` | `#F8FAFC` | Nền toàn bộ màn hình Scaffold |
| `AppColors.surface` | `#FFFFFF` | Nền thẻ Card, BottomSheet, Dialog, Input container |
| `AppColors.border` | `#E2E8F0` | Đường viền 1px phân tách các phần tử và thẻ Card |
| `AppColors.textPrimary` | `#1E293B` | Chữ tiêu đề, tên sản phẩm, số tiền (Slate 800) |
| `AppColors.textSecondary` | `#64748B` | Chữ mô tả, nhãn phụ, ngày giờ (Slate 500) |
| `AppColors.textMuted` | `#94A3B8` | Hint text, icon chưa kích hoạt, placeholder (Slate 400) |
| `AppColors.success` | `#10B981` | Còn hàng, đã thanh toán, hoàn thành, số dư dương |
| `AppColors.warning` | `#F59E0B` | Cảnh báo sắp hết hàng, đơn chờ xử lý, lưu ý |
| `AppColors.error` | `#EF4444` | Hết hàng, hủy đơn, lỗi mạng, xóa phần tử, số dư nợ |

> [!IMPORTANT]
> **Quy tắc tương thích Flutter 3.47+**:
> TUYỆT ĐỐI KHÔNG dùng `.withOpacity(...)` vì gây cảnh báo deprecation.
> LUÔN LUÔN dùng `.withValues(alpha: 0.12)` khi cần độ trong suốt.

---

## 3. HỆ THỐNG KIỂU CHỮ (TYPOGRAPHY HIERARCHY)
* **Font Family**: Google Fonts `Inter` áp dụng toàn hệ thống (được cấu hình trong `AppTheme.lightTheme`).
* **Thang kích thước chuẩn**:
  - `Display / KPI`: `26 - 32px`, `FontWeight.bold` (dành cho số tiền tổng doanh thu trên Dashboard).
  - `Headline`: `18 - 20px`, `FontWeight.bold` (dành cho tiêu đề màn hình, tên chi nhánh).
  - `Title`: `15 - 16px`, `FontWeight.w600` (dành cho tên sản phẩm, tiêu đề khối nhóm).
  - `Body / Standard`: `13 - 14px`, `FontWeight.normal` (dành cho mô tả, nội dung bảng kê, ô nhập liệu).
  - `Caption / Subtext`: `11 - 12px`, `FontWeight.w500` (dành cho mã SKU, ngày giờ, số lượng tồn).
  - `Price Tag`: `15 - 16px`, `FontWeight.bold`, màu `AppColors.primaryLight` hoặc `AppColors.textPrimary`.

---

## 4. LƯỚI BỐ CỤC & KHOẢNG CÁCH (8PT SPACING GRID)
* Mọi khoảng cách padding và margin phải là bội số của 4 hoặc 8:
  - `4px`: Khoảng cách giữa icon và label nhỏ.
  - `8px`: Khoảng cách giữa các chip, khoảng cách giữa title và subtitle.
  - `12px`: Khoảng cách giữa các item trong danh sách (List item separator).
  - `16px`: Padding chuẩn viền màn hình (Horizontal Screen Padding).
  - `20 - 24px`: Khoảng cách giữa các khối chức năng độc lập (Section Spacing).
  - `32px`: Khoảng cách đỉnh đầu hoặc cuối form nhập liệu.

---

## 5. QUY CHUẨN THIẾT KẾ CÁC COMPONENT GIAO DIỆN (UI COMPONENTS)

### 5.1. Thẻ Card & Container
* **Border Radius**: Luôn bo góc `14px` đến `18px` (`BorderRadius.circular(16)`).
* **Border**: Bo viền mảnh `Border.all(color: AppColors.border, width: 1)`.
* **Elevation**: Đặt `elevation: 0`, tạo chiều sâu bằng màu nền `Colors.white` nổi trên nền xám `#F8FAFC`.

### 5.2. Nút bấm (Buttons)
* **Chiều cao chuẩn (Height)**: `48px` đến `52px` để ngón tay dễ dàng bấm trúng.
* **Bo góc**: `BorderRadius.circular(12)`.
* **Trạng thái Loading**: Khi đang gửi API, nút bấm phải chuyển sang hiển thị vòng tròn xoay `CircularProgressIndicator(strokeWidth: 2.5, color: Colors.white)` và vô hiệu hóa click lặp lại (`onPressed: null`).
* **Nút bấm hủy / xóa nguy hiểm**: Sử dụng OutlinedButton viền đỏ `AppColors.error` kèm icon cảnh báo.

### 5.3. Ô nhập liệu (Input Fields)
* **Cấu trúc**: Luôn có Label rõ ràng ở trên đầu ô nhập (`fontSize: 13, fontWeight: FontWeight.w600`).
* **Container**: Nền trắng `Colors.white`, viền mảnh `AppColors.border`.
* **Icon bổ trợ**: Prefix icon kích thước `20px` với màu `AppColors.textSecondary` để người dùng nhận diện nhanh loại dữ liệu (User, Password, Phone, Money, Barcode...).
* **Clear Button**: Các ô tìm kiếm phải có nút icon dấu `x` để xóa nhanh từ khóa khi có text.

### 5.4. Thẻ Trạng Thái & Huy Hiệu (Badges & Chips)
* **Kích thước**: Nhỏ gọn, `padding: EdgeInsets.symmetric(horizontal: 8, vertical: 3)`.
* **Bo góc**: `BorderRadius.circular(6)` đến `BorderRadius.circular(8)`.
* **Quy tắc màu**: Nền nhạt (`color.withValues(alpha: 0.12)`) và chữ đậm cùng tone màu (`fontWeight: FontWeight.w600`).
  - *Còn hàng*: Nền xanh nhạt, chữ xanh lá `AppColors.success`.
  - *Sắp hết hàng / Tồn ít*: Nền cam nhạt, chữ cam `AppColors.warning`.
  - *Hết hàng*: Nền đỏ nhạt, chữ đỏ `AppColors.error`.

### 5.5. Danh Sách & Ảnh Sản Phẩm (Product Lists)
* **Thumbnail Sản phẩm**:
  - Kích thước vuông: `64x64px` hoặc `72x72px`, bo góc `10px`.
  - Bắt buộc dùng `CachedNetworkImage` kèm `placeholder` Shimmer/Container xám nhạt và `errorWidget` hiển thị icon ảnh mặc định nếu URL ảnh bị rỗng hoặc lỗi.
* **Cấu trúc dòng**:
  - Tên sản phẩm tối đa 2 dòng, tự động cắt ngắn `TextOverflow.ellipsis`.
  - Dòng dưới gồm: Mã Barcode/SKU bên trái, Giá tiền in đậm và Huy hiệu tồn kho bên phải.

### 5.6. Bảng Điều Khiển BottomSheet (Modal & Bottom Drawer)
* Bo góc trên: `BorderRadius.vertical(top: Radius.circular(24))`.
* Luôn có **Thanh kéo (Drag Handle)** ở trên cùng:
  ```dart
  Container(
    width: 40,
    height: 4,
    margin: const EdgeInsets.symmetric(vertical: 10),
    decoration: BoxDecoration(
      color: AppColors.border,
      borderRadius: BorderRadius.circular(2),
    ),
  )
  ```
* Bọc nội dung trong `SafeArea` để không bị đè bởi thanh điều hướng hệ thống của điện thoại.

### 5.7. Trạng Thái Chờ & Rỗng (Loading & Empty States)
* **Loading**: Không dùng vòng tròn xoay đơn điệu giữa màn hình trắng. Hãy ưu tiên dùng hiệu ứng Shimmer (xương khung giao diện) tương ứng với kích thước các thẻ Card.
* **Empty State**: Khi danh sách rỗng, phải hiển thị:
  1. Icon minh họa lớn (`size: 56 - 64px`) màu mờ `AppColors.textMuted.withValues(alpha: 0.4)`.
  2. Dòng thông báo chính rõ ràng (Ví dụ: *"Chưa có đơn hàng nào hôm nay"*).
  3. Dòng gợi ý phụ (Ví dụ: *"Nhấn nút bên dưới để tạo đơn bán hàng đầu tiên"*).
  4. Nút bấm hành động nhanh (Call to Action).

### 5.8. Dữ Liệu Tài Chính & Hiển Thị Số Tiền
* **Quy chuẩn**: Luôn gọi qua `Formatters.formatVND(value)` để đảm bảo số tiền hiển thị đúng chuẩn Việt Nam (`1.500.000 ₫`).
* **Tính năng riêng tư (Privacy Toggle)**: Các màn hình hiển thị doanh thu, lãi lỗ phải có biểu tượng con mắt để chuyển đổi hiển thị số tiền thành `****** ₫` khi người dùng ở nơi công cộng.

---

## 6. PHẢN HỒI XÚC GIÁC & TƯƠNG TÁC (MICRO-INTERACTIONS)
* **Kéo để làm mới (Pull-to-Refresh)**: Mọi màn hình danh sách và Dashboard bắt buộc phải bọc trong `RefreshIndicator`.
* **Phản hồi âm thanh / Rung**:
  - Khi quét thành công mã vạch sản phẩm: phát tiếng Beep nhẹ và rung xúc giác (HapticFeedback.mediumImpact).
  - Khi quét sai mã hoặc cảnh báo tồn kho: rung kép (HapticFeedback.vibrate).
  - Khi bấm nút thanh toán / xác nhận: rung nhẹ xác nhận (HapticFeedback.lightImpact).

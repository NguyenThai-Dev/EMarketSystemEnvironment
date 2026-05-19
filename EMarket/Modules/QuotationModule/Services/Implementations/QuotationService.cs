using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EMarket.Events.Interfaces;
using EMarket.Models;
using EMarket.Modules.CustomerModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.QuotationModule.DTOs;
using EMarket.Modules.QuotationModule.Services.Interfaces;
using EMarket.Modules.SalesModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.QuotationModule.Services.Implementations
{
    public class QuotationService : IQuotationService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IBranchService _branchService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAddressService _customerAddressService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly string _connStr;
        private readonly IEmailService _emailService;
        private readonly DateTime defaultDate = new DateTime(2000, 1, 1);

        public QuotationService(
            EMarket_DBEntities db,
            IBranchService branchService,
            ICustomerService customerService,
            IUserService userService,
            IProductService productService,
            IEmailService emailService,
            ICustomerAddressService customerAddressService)
        {
            _db = db;
            _branchService = branchService;
            _customerService = customerService;
            _userService = userService;
            _productService = productService;
            _customerAddressService = customerAddressService;
            _emailService = emailService;
            _connStr = ConfigurationManager
          .ConnectionStrings["EMarket_Connections"]
          .ConnectionString;
        }

        #region CREATE

        public async Task<int> CreateQuotationAsync(QuotationDTO dto)
        {
            // Sử dụng Transaction để đảm bảo tính toàn vẹn (Lưu Header + Detail cùng lúc)
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Tạo Header (Bảng Quotations)
                    var quotation = new Quotation
                    {
                        quotation_code = GenerateCode(), // Hàm sinh mã BG-XXX của bạn
                        branch_id = dto.BranchId,
                        customer_id = dto.CustomerId,
                        user_id = dto.UserId,

                        issue_date = dto.IssueDate,
                        expiry_date = dto.ExpiryDate,

                        total_amount = dto.TotalAmount,       // Đã bao gồm VAT từ JS gửi lên
                        discount_amount = dto.DiscountAmount, // Tổng giảm giá (Điểm + Tay)
                        manual_discount = dto.ManualDiscount, // Chỉ chứa tiền quy đổi điểm (nếu có)
                        discount_reason = dto.DiscountReason,

                        // FinalAmount trong DB thường là Computed Column hoặc tính toán
                        // Nếu DB bạn có cột này thì gán:
                        final_amount = dto.TotalAmount - dto.DiscountAmount,

                        status = "Draft",
                        note = dto.Note,
                        created_at = DateTime.Now
                    };

                    _db.Quotations.Add(quotation);
                    await _db.SaveChangesAsync(); // Lưu để lấy ID

                    // 2. Tạo Details (Bảng QuotationDetails)
                    if (dto.Details != null && dto.Details.Count > 0)
                    {
                        var detailsEntities = new List<QuotationDetail>();

                        foreach (var d in dto.Details)
                        {
                            var detail = new QuotationDetail
                            {
                                quotation_id = quotation.quotation_id,
                                product_id = d.ProductId,
                                quantity = d.Quantity,
                                unit_price = d.UnitPrice,
                                discount = d.Discount,
                                total_price = d.TotalPrice, // Đã gồm VAT dòng
                                note = d.Note
                            };
                            detailsEntities.Add(detail);
                        }

                        _db.QuotationDetails.AddRange(detailsEntities); // AddRange nhanh hơn Add từng cái
                        await _db.SaveChangesAsync();
                    }

                    // 3. Commit Transaction
                    trans.Commit();
                    return quotation.quotation_id;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception("Lỗi khi lưu báo giá: " + ex.Message);
                }
            }
        }
        #endregion

        #region GET BY ID

        public async Task<QuotationDTO> GetQuotationByIdAsync(int id)
        {
            var quotation = await _db.Quotations
                .Include("QuotationDetails")
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.quotation_id == id);

            if (quotation == null)
                return null;

            var dto = MapCoreToDTO(quotation);

            await EnrichQuotationDTOAsync(dto);

            return dto;
        }

        #endregion

        #region LIST / FILTER

        public async Task<List<QuotationDTO>> GetAllQuotationsAsync(
    string keyword,
    int? branchId,
    string status,
    DateTime? fromDate,
    DateTime? toDate)
        {
            var query = _db.Quotations
                .Include("QuotationDetails")
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.quotation_code.Contains(keyword));

            if (branchId.HasValue)
                query = query.Where(x => x.branch_id == branchId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.status == status);

            if (fromDate.HasValue)
                query = query.Where(x => x.issue_date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.issue_date <= toDate.Value);

            var quotations = await query
                .OrderByDescending(x => x.created_at)
                .ToListAsync();

            var result = quotations.Select(MapCoreToDTO).ToList();

            foreach (var dto in result)
            {
                await EnrichQuotationDTOAsync(dto);
            }

            return result;
        }

        #endregion

        #region UPDATE

        public async Task<bool> UpdateQuotationAsync(QuotationDTO dto)
        {
            // Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Lấy Header cũ (Kèm Details để xóa)
                    var quotation = await _db.Quotations
                        .Include("QuotationDetails") // Include để xóa details cũ
                        .FirstOrDefaultAsync(x => x.quotation_id == dto.QuotationId);

                    if (quotation == null)
                    {
                        return false;
                    }

                    // Chỉ cho phép sửa nếu trạng thái chưa chốt hoặc chưa hủy (Optional check)
                    if (quotation.status == "Converted" || quotation.status == "Cancelled")
                    {
                        return false;
                    }

                    // 2. Cập nhật thông tin Header
                    quotation.branch_id = dto.BranchId;
                    quotation.customer_id = dto.CustomerId;
                    // quotation.user_id = dto.UserId; // Thường người sửa là người cập nhật cuối, không nhất thiết đổi người tạo ban đầu

                    quotation.issue_date = dto.IssueDate;
                    quotation.expiry_date = dto.ExpiryDate;

                    // Cập nhật các trường tài chính quan trọng
                    quotation.total_amount = dto.TotalAmount;       // Đã bao gồm VAT
                    quotation.discount_amount = dto.DiscountAmount; // Tổng giảm giá
                    quotation.final_amount = dto.FinalAmount;       // Số tiền cuối cùng

                    // Cập nhật các trường mới bổ sung
                    quotation.manual_discount = dto.ManualDiscount;
                    quotation.discount_reason = dto.DiscountReason;

                    quotation.note = dto.Note;
                    quotation.updated_at = DateTime.Now; // Cập nhật thời gian sửa

                    // 3. Xử lý Details (Xóa cũ -> Thêm mới)

                    // Xóa hết chi tiết cũ
                    if (quotation.QuotationDetails != null && quotation.QuotationDetails.Count > 0)
                    {
                        _db.QuotationDetails.RemoveRange(quotation.QuotationDetails);
                    }

                    // Thêm chi tiết mới
                    if (dto.Details != null && dto.Details.Count > 0)
                    {
                        var newDetails = new List<QuotationDetail>();
                        foreach (var d in dto.Details)
                        {
                            var detail = new QuotationDetail
                            {
                                quotation_id = quotation.quotation_id, // Link với Header đang sửa
                                product_id = d.ProductId,
                                quantity = d.Quantity,
                                unit_price = d.UnitPrice,
                                discount = d.Discount,
                                total_price = d.TotalPrice, // Đã gồm VAT dòng
                                note = d.Note
                            };
                            newDetails.Add(detail);
                        }

                        // AddRange hiệu năng tốt hơn Add trong vòng lặp
                        _db.QuotationDetails.AddRange(newDetails);
                    }

                    // 4. Lưu và Commit
                    await _db.SaveChangesAsync();
                    trans.Commit();

                    // CHỈ gửi lại email báo giá mới nếu báo giá này đã từng được 'Sent'
                    if (quotation.status == "Sent")
                    {
                        await SendQuotationEmailAsync(quotation);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    // Log lỗi tại đây nếu cần
                    throw new Exception("Lỗi khi cập nhật báo giá: " + ex.Message);
                }
            }
        }

        #endregion

        #region STATUS

        public async Task<bool> ChangeStatusAsync(int id, string newStatus)
        {
            // 1. Load quotation + details (KHÔNG include Customer)
            var q = await _db.Quotations
                .Include(x => x.QuotationDetails)
                .FirstOrDefaultAsync(x => x.quotation_id == id);

            if (q == null) return false;

            // 2. Validate trạng thái
            if (q.status == "Converted") return false;
            if (q.status == "Cancelled" && newStatus != "Draft") return false;

            // 3. Logic gửi email khi SENT
            if (newStatus == "Sent")
            {
                await SendQuotationEmailAsync(q);
            }

            // 4. Update trạng thái
            q.status = newStatus;
            //q.status = "Draft";
            q.updated_at = DateTime.Now;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteQuotationAsync(int id)
        {
            var q = await _db.Quotations.FirstOrDefaultAsync(x => x.quotation_id == id);
            if (q == null) return false;

            // Chỉ cho phép xóa Draft
            if (q.status != "Draft") return false;

            // Xóa chi tiết trước (nếu không có Cascade Delete)
            var details = _db.QuotationDetails.Where(x => x.quotation_id == id);
            _db.QuotationDetails.RemoveRange(details);

            _db.Quotations.Remove(q);
            await _db.SaveChangesAsync();
            return true;
        }

        #endregion

        #region CONVERT TO ORDER (CHUẨN MODULE)

        public async Task<CheckoutResultDTO> ConvertQuotationToOrderAsync(int quotationId, int userId)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@quotation_id", quotationId);
                    p.Add("@user_id", userId);

                    // Dùng QuerySingleAsync vì SP này trả về SELECT ...
                    var result = await conn.QuerySingleAsync<dynamic>(
                        "sp_Quotation_ConvertToOrder", p, commandType: CommandType.StoredProcedure
                    );

                    return new CheckoutResultDTO
                    {
                        Success = true,
                        OrderId = (int)result.OrderId,
                        Message = result.Message
                    };
                }
                catch (SqlException ex)
                {
                    // Bắt lỗi nghiệp vụ (ví dụ báo giá đã convert rồi)
                    return new CheckoutResultDTO { Success = false, Message = ex.Message };
                }
                catch (Exception ex)
                {
                    return new CheckoutResultDTO { Success = false, Message = "Lỗi hệ thống: " + ex.Message };
                }
            }
        }
        #endregion

        #region PRIVATE

        private string GenerateCode()
        {
            return "BG-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
        }

        private async Task SendQuotationEmailAsync(Quotation q)
        {
            if (!q.customer_id.HasValue || q.customer_id.Value <= 0)
                return;

            string customerEmail = await _customerService.GetCustomerEmailAsync(q.customer_id.Value);

            if (!string.IsNullOrWhiteSpace(customerEmail))
            {
                try
                {
                    string subject = $"[EMarket] Báo giá đơn hàng #{q.quotation_code}";
                    string htmlBody = await GenerateQuotationHtml(q);

                    await _emailService.SendAsync(customerEmail, subject, htmlBody);
                }
                catch (Exception ex)
                {
                    // Nếu thực tế email không phải là bước bắt buộc tuyệt đối (tức là không muốn fail transaction),
                    // có thể chỉ log lỗi ra. Hiện tại giữ nguyên ý định cũ là throw để báo lỗi.
                    throw new Exception($"Không thể gửi email tới khách hàng ({customerEmail}): {ex.Message}", ex);
                }
            }
        }

        private async Task EnrichQuotationDTOAsync(QuotationDTO dto)
        {
            // Branch
            var branch = await _branchService.GetBranchByIdAsync(dto.BranchId);
            if (branch != null)
            {
                dto.BranchName = branch.Name;
            }

            // Customer
            if (dto.CustomerId.HasValue)
            {
                var customer = await _customerService.GetCustomerByIdAsync(dto.CustomerId.Value);
                var customerAddress = await _customerAddressService.GetDefaultCustomerAddressAsync(dto.CustomerId.Value);
                if (customer != null)
                {
                    dto.CustomerName = customer.FullName;
                    dto.CustomerPhone = customer.Phone;
                    dto.CustomerAddress = customerAddress.FullAddress;
                    ;
                }
            }
            else
            {
                dto.CustomerName = "Khách lẻ";
            }

            // User
            var user = await _userService.GetUserByIdAsync(dto.UserId);
            if (user != null)
            {
                dto.CreatorName = user.FullName;
            }

            // Product (bulk-safe)
            var productIds = dto.Details.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productService.GetProductsByIdsAsync(productIds);

            foreach (var d in dto.Details)
            {
                var p = products.FirstOrDefault(x => x.ProductId == d.ProductId);
                if (p != null)
                {
                    d.ProductName = p.Name;
                    d.ProductImage = p.Image;
                    d.Unit = p.Unit;
                }
            }
        }


        private QuotationDTO MapCoreToDTO(Quotation q)
        {
            return new QuotationDTO
            {
                QuotationId = q.quotation_id,
                QuotationCode = q.quotation_code,
                BranchId = q.branch_id,
                CustomerId = q.customer_id,
                UserId = q.user_id,
                IssueDate = q.issue_date ?? DateTime.Now,
                ExpiryDate = q.expiry_date,
                TotalAmount = q.total_amount,
                DiscountAmount = q.discount_amount ?? 0,
                Status = q.status,
                Note = q.note,
                ManualDiscount = q.manual_discount ?? 0,
                DiscountReason = q.discount_reason,
                ConvertedOrderId = q.converted_order_id,
                Details = q.QuotationDetails.Select(d => new QuotationDetailDTO
                {
                    DetailId = d.detail_id,
                    ProductId = d.product_id,
                    Quantity = d.quantity,
                    UnitPrice = d.unit_price,
                    Discount = d.discount ?? 0,
                    TotalPrice = d.total_price,
                    Note = d.note
                }).ToList()
            };
        }

        private async Task<string> GenerateQuotationHtml(Quotation q)
        {
            // 1. Lấy danh sách ProductId & map tên
            var productIds = q.QuotationDetails?
                .Select(x => x.product_id)
                .Distinct()
                .ToList() ?? new List<int>();

            var productNameDic = productIds.Any()
                ? await _productService.GetProductNamesByIdsAsync(productIds)
                : new Dictionary<int, string>();

            var sb = new StringBuilder();

            sb.Append(@"
<div style='background:#f4f6f8; padding:20px 0; font-family:Arial, Helvetica, sans-serif;'>
  <div style='max-width:600px; margin:0 auto; background:#ffffff; border-radius:6px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.05);'>

    <!-- HEADER -->
    <div style='background:#0d6efd; color:#ffffff; padding:16px 20px;'>
      <h2 style='margin:0; font-size:20px;'>EMarket - Báo giá bán hàng</h2>
      <p style='margin:4px 0 0; font-size:13px;'>Giải pháp bán lẻ & chuỗi siêu thị</p>
    </div>

    <!-- BODY -->
    <div style='padding:20px; color:#333;'>
      <p>Kính chào Quý khách,</p>

      <p>
        EMarket trân trọng gửi đến Quý khách báo giá
        <strong style='color:#0d6efd;'>#" + q.quotation_code + @"</strong>.
      </p>

      <p style='font-size:13px; color:#666;'>
        Ngày tạo: <strong>" + (q.issue_date?.ToString("dd/MM/yyyy") ?? "") + @"</strong>
        &nbsp;|&nbsp;
        Hiệu lực đến: <strong>" + q.expiry_date.ToString("dd/MM/yyyy") + @"</strong>
      </p>

      <!-- TABLE -->
      <table style='width:100%; border-collapse:collapse; margin-top:15px; font-size:13px;'>
        <thead>
          <tr style='background:#f2f2f2;'>
            <th style='border:1px solid #ddd; padding:8px; text-align:left;'>Sản phẩm</th>
            <th style='border:1px solid #ddd; padding:8px; text-align:center;'>SL</th>
            <th style='border:1px solid #ddd; padding:8px; text-align:right;'>Đơn giá</th>
            <th style='border:1px solid #ddd; padding:8px; text-align:right;'>Thành tiền</th>
          </tr>
        </thead>
        <tbody>");

            if (q.QuotationDetails != null && q.QuotationDetails.Any())
            {
                foreach (var item in q.QuotationDetails)
                {
                    var productName = productNameDic.TryGetValue(item.product_id, out var name)
                        ? name
                        : $"SP#{item.product_id}";

                    var totalPrice = item.total_price > 0
                        ? item.total_price
                        : item.unit_price * item.quantity;

                    sb.Append(@"
          <tr>
            <td style='border:1px solid #ddd; padding:8px;'>" + productName + @"</td>
            <td style='border:1px solid #ddd; padding:8px; text-align:center;'>" + item.quantity + @"</td>
            <td style='border:1px solid #ddd; padding:8px; text-align:right;'>" + item.unit_price.ToString("N0") + @"</td>
            <td style='border:1px solid #ddd; padding:8px; text-align:right;'>" + totalPrice.ToString("N0") + @"</td>
          </tr>");
                }
            }

            sb.Append(@"
        </tbody>
      </table>

      <!-- TOTAL -->
      <div style='margin-top:15px; text-align:right;'>
        <p style='margin:4px 0;'>Tạm tính: <strong>" + q.total_amount.ToString("N0") + @" VNĐ</strong></p>");

            if (q.discount_amount.HasValue && q.discount_amount > 0)
            {
                sb.Append(@"
        <p style='margin:4px 0; color:#dc3545;'>Giảm giá: -" + q.discount_amount.Value.ToString("N0") + @" VNĐ</p>");
            }

            sb.Append(@"
        <p style='margin:8px 0; font-size:16px;'>
          <strong style='color:#0d6efd;'>THANH TOÁN: " + q.final_amount.ToString("N0") + @" VNĐ</strong>
        </p>
      </div>

      <p style='margin-top:20px;'>
        Nếu Quý khách cần điều chỉnh hoặc xác nhận báo giá, vui lòng phản hồi email này
        hoặc liên hệ trực tiếp với chúng tôi.
      </p>

      <p style='margin-top:20px;'>
        Trân trọng,<br>
        <strong>EMarket Team</strong>
      </p>
    </div>

    <!-- FOOTER -->
    <div style='background:#f8f9fa; padding:12px 20px; font-size:12px; color:#777; text-align:center;'>
      © " + DateTime.Now.Year + @" EMarket. Hotline: 1900-xxxx | support@emarket.vn
    </div>

  </div>
</div>");

            return sb.ToString();
        }


        #endregion
    }

}
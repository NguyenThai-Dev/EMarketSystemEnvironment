using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EMarket.Events.Interfaces;
using EMarket.Models;
using EMarket.Modules.CustomerModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.SalesModule.DTOs;
using EMarket.Modules.SalesModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;
using Order = EMarket.Models.Order;

namespace EMarket.Modules.SalesModule.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly EMarket_DBEntities _db;
        private readonly string _connectionString;
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IUserContext _userContext;
        private readonly IOrderRealtimeService _orderRealtimeService;
        private readonly IProductService _productService;
        private readonly DateTime defaultDate = new DateTime(2000, 1, 1);

        public OrderService(EMarket_DBEntities db, ICustomerService customerService, IUserService userService, IOrderRealtimeService orderRealtimeService, IUserContext userContext, IProductService productService)
        {
            _db = db;
            _connectionString = ConfigurationManager
           .ConnectionStrings["EMarket_Connections"]
           .ConnectionString;
            _userService = userService;
            _customerService = customerService;
            _orderRealtimeService = orderRealtimeService;
            _userContext = userContext;
            _productService = productService;
        }

        public async Task<List<OrderDTO>> GetAllOrdersAsync()
        {
            return await _db.Orders
                .Select(o => new OrderDTO
                {
                    OrderId = o.order_id,
                    CustomerId = o.customer_id,
                    UserId = o.user_id,
                    BranchId = o.branch_id,
                    OrderDate = o.order_date ?? defaultDate,
                    Status = o.status,
                    TotalAmount = o.total_amount ?? 0,
                    DeliveryAddressId = o.delivery_address_id
                }).ToListAsync();
        }

        public async Task<List<OrderDTO>> GetOrdersByBranchIdAsync(int? branchId, DateTime? fromDate, DateTime? toDate)
        {
            // 1. Khởi tạo query
            var query = _db.Orders.AsNoTracking().AsQueryable();

            // 2. Lọc chi nhánh
            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(o => o.branch_id == branchId.Value);
            }

            // 3. Lọc Từ ngày
            if (fromDate.HasValue)
            {
                var fDate = fromDate.Value.Date;
                query = query.Where(o => o.order_date >= fDate);
            }

            // 4. Lọc Đến ngày (Logic < ngày hôm sau)
            if (toDate.HasValue)
            {
                var tDate = toDate.Value.Date.AddDays(1);
                query = query.Where(o => o.order_date < tDate);
            }

            // 5. Select DTO
            return await query
                .OrderByDescending(o => o.order_date) // Nên sắp xếp mới nhất lên đầu
                .Select(o => new OrderDTO
                {
                    OrderId = o.order_id,
                    OrderDate = o.order_date ?? defaultDate,
                    TotalAmount = o.total_amount ?? 0,
                })
                .ToListAsync();
        }

        public async Task<(int total, int filtered, List<OrderDTO> data)> GetOrdersDataTableAsync(
    int draw, int start, int length, int? userId, int? branchId,
    string status, DateTime? fromDate, DateTime? toDate, string keyword)
        {
            // 1. Khởi tạo Query (Dùng IQueryable để tránh lỗi ép kiểu DbQuery)
            IQueryable<Order> baseQuery = _db.Orders.AsNoTracking();

            // 2. Lấy Total Count (Có thể cache số này nếu bảng quá lớn > 1 triệu dòng)
            var total = await _db.Orders.CountAsync();

            // 3. Apply Filters
            if (userId.HasValue) baseQuery = baseQuery.Where(x => x.user_id == userId);
            if (branchId.HasValue) baseQuery = baseQuery.Where(x => x.branch_id == branchId);
            if (!string.IsNullOrEmpty(status)) baseQuery = baseQuery.Where(x => x.status == status);

            if (fromDate.HasValue)
            {
                var f = fromDate.Value.Date;
                baseQuery = baseQuery.Where(x => x.order_date >= f);
            }
            if (toDate.HasValue)
            {
                var t = toDate.Value.Date.AddDays(1);
                baseQuery = baseQuery.Where(x => x.order_date < t);
            }

            // Tối ưu Search Keyword: Không dùng ToString() với ID
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                if (int.TryParse(keyword, out int id))
                {
                    // Nếu keyword là số -> Search theo ID (Ăn Index, siêu nhanh)
                    baseQuery = baseQuery.Where(x => x.order_id == id);
                }
                else
                {
                    // Nếu keyword là chữ -> Search theo Status hoặc cột text khác
                    baseQuery = baseQuery.Where(x => x.status.Contains(keyword));
                }
            }

            // 4. Lấy Filtered Count
            var filtered = await baseQuery.CountAsync();

            // 5. Lấy Data chính (Paging) - CHỈ LẤY BẢNG ORDER, KHÔNG JOIN
            var orderData = await baseQuery
                .OrderByDescending(x => x.order_date)
                .ThenByDescending(x => x.order_id)
                .Skip(start)
                .Take(length)
                .ToListAsync();

            // Nếu không có data thì return luôn cho nhẹ
            if (!orderData.Any()) return (total, filtered, new List<OrderDTO>());

            // ==========================================================
            // BATCH LOADING (Tải dữ liệu liên quan theo lô ID)
            // ==========================================================

            // 6. Gom các ID cần thiết
            var orderIds = orderData.Select(x => x.order_id).ToList();
            var uIds = orderData.Select(x => x.user_id).Where(x => x.HasValue).Select(x => x.Value).Distinct().ToList();
            var cIds = orderData.Select(x => x.customer_id).Distinct().ToList();

            // 7. Fetch dữ liệu liên quan (Chạy tuần tự với DB Context hiện tại để tránh lỗi Threading của EF6)
            // Tuy nhiên nhờ filter theo ID (WHERE IN) nên tốc độ vẫn cực nhanh (vài ms)

            // Lấy Details
            var detailsList = await _db.OrderDetails.AsNoTracking()
                .Where(d => orderIds.Contains(d.order_id))
                .Select(d => new
                {
                    d.order_id,
                    d.product_id,
                    d.quantity,
                    d.unit_price,
                    d.discount,
                    ProductName = d.Product.name // Join nhẹ lấy tên SP
                })
                .ToListAsync();

            // Lấy Payments

            var paymentsList = await _db.Payments.AsNoTracking()
                .Where(p => orderIds.Contains(p.order_id))
                .Select(p => new { p.order_id, p.payment_method, p.status })
                .ToListAsync();

            // 8. Gọi External Services (User & Customer) - Chạy SONG SONG được vì khác Context/Service
            var userTask = _userService.GetUserDictAsync(uIds);
            var customerTask = _customerService.GetCustomerNameDictAsync(cIds);

            await Task.WhenAll(userTask, customerTask);

            var userDict = userTask.Result;
            var customerDict = customerTask.Result;

            // 9. MAP DATA IN MEMORY (Ghép dữ liệu lại trong RAM)
            var result = orderData.Select(x => new OrderDTO
            {
                OrderId = x.order_id,
                CustomerId = x.customer_id,
                UserId = x.user_id,
                BranchId = x.branch_id,
                OrderDate = x.order_date ?? DateTime.Now,
                Status = x.status,
                TotalAmount = x.total_amount ?? 0,
                DeliveryAddressId = x.delivery_address_id,

                // Map tên từ Dictionary
                UserName = userDict.TryGetValue(x.user_id ?? 0, out var uName) ? uName.FullName : "N/A",
                CustomerName = customerDict.TryGetValue(x.customer_id, out var cName) ? cName : "Khách vãng lai",

                // Map Payment từ List đã lấy
                PaymentMethod = paymentsList.FirstOrDefault(p => p.order_id == x.order_id)?.payment_method ?? "N/A",
                PaymentStatus = paymentsList.FirstOrDefault(p => p.order_id == x.order_id)?.status ?? "Unpaid",

                // Map Order Details từ List đã lấy
                OrderDetails = detailsList
                    .Where(d => d.order_id == x.order_id)
                    .Select(d => new OrderDetailDTO
                    {
                        OrderDetailId = 0, // Không cần thiết hiển thị ở table
                        OrderId = d.order_id,
                        ProductId = d.product_id,
                        ProductName = d.ProductName,
                        Quantity = d.quantity,
                        UnitPrice = d.unit_price,
                        Discount = d.discount
                    }).ToList()
            }).ToList();

            return (total, filtered, result);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
        {
            var order = await _db.Orders
                .AsNoTracking()
                .Where(x => x.order_id == orderId)
                .Select(x => new OrderDTO
                {
                    OrderId = x.order_id,
                    CustomerId = x.customer_id,
                    UserId = x.user_id,
                    BranchId = x.branch_id,
                    OrderDate = x.order_date ?? defaultDate,
                    Status = x.status,
                    TotalAmount = x.total_amount ?? 0,
                    DeliveryAddressId = x.delivery_address_id,

                    PaymentMethod = x.Payments.Select(p => p.payment_method).FirstOrDefault() ?? "N/A",
                    PaymentStatus = x.Payments.Select(p => p.status).FirstOrDefault() ?? "Unpaid",

                    OrderDetails = x.OrderDetails.Select(d => new OrderDetailDTO
                    {
                        OrderDetailId = d.order_detail_id,
                        OrderId = d.order_id,
                        ProductId = d.product_id,
                        Quantity = d.quantity,
                        UnitPrice = d.unit_price,
                        Discount = d.discount,
                        ProductName = d.Product.name
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
                return null;

            /* ==== MAP CUSTOMER NAME (THROUGH SERVICE) ==== */
            if (order.CustomerId > 0)
            {
                var dict = await _customerService
                    .GetCustomerNameDictAsync(new List<int> { order.CustomerId });

                order.CustomerName = dict.TryGetValue(order.CustomerId, out var name)
                    ? name
                    : "Khách vãng lai";
            }
            else
            {
                order.CustomerName = "Khách vãng lai";
            }

            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string connectionId)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(x => x.order_id == orderId);
            if (order == null) return false;

            order.status = status;
            await _db.SaveChangesAsync();

            await _orderRealtimeService.NotifyOrderStatusChangedAsync(
                order.order_id,
                status,
                order.branch_id ?? 1,
                connectionId
            );

            return true;
        }

        public async Task<int> CreateOrderAsync(OrderDTO dto)
        {
            try
            {
                var entity = new Order
                {
                    customer_id = dto.CustomerId,
                    user_id = dto.UserId,
                    branch_id = dto.BranchId,
                    order_date = dto.OrderDate,
                    status = dto.Status,
                    total_amount = dto.TotalAmount,
                    delivery_address_id = dto.DeliveryAddressId
                };

                _db.Orders.Add(entity);
                await _db.SaveChangesAsync();

                await _orderRealtimeService.NotifyOrderCreatedAsync(
                    entity.order_id,
                    entity.status,
                    entity.branch_id ?? 1
                );

                return entity.order_id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating order: " + ex.Message);
            }
        }

        public async Task<bool> UpdateOrderAsync(OrderDTO dto)
        {
            try
            {
                var entity = await _db.Orders.FindAsync(dto.OrderId);
                if (entity == null) return false;

                entity.customer_id = dto.CustomerId;
                entity.user_id = dto.UserId;
                entity.branch_id = dto.BranchId;
                entity.status = dto.Status;
                entity.total_amount = dto.TotalAmount;
                entity.delivery_address_id = dto.DeliveryAddressId;

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating order: " + ex.Message);
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                var entity = await _db.Orders.FindAsync(orderId);
                if (entity == null) return false;

                _db.Orders.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting order: " + ex.Message);
            }
        }

        public async Task<List<OrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var details = await (
                from d in _db.OrderDetails
                join o in _db.Orders on d.order_id equals o.order_id
                where d.order_id == orderId
                select new OrderDetailDTO
                {
                    OrderDetailId = d.order_detail_id,
                    OrderId = d.order_id,
                    CustomerId = o.customer_id,
                    ProductId = d.product_id,
                    Quantity = d.quantity,
                    UnitPrice = d.unit_price,
                    Discount = d.discount,
                    QuantityReturned = d.quantity_returned
                }
            ).ToListAsync();

            if (!details.Any())
                return details;

            var productIds = details
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            var productNameMap = await _productService
                .GetProductNamesByIdsAsync(productIds);

            foreach (var item in details)
            {
                if (productNameMap.TryGetValue(item.ProductId, out var name))
                {
                    item.ProductName = name;
                }
            }

            return details;
        }



        public async Task<int> CreateOrderDetailAsync(OrderDetailDTO dto)
        {
            try
            {
                var entity = new OrderDetail
                {
                    order_id = dto.OrderId,
                    product_id = dto.ProductId,
                    quantity = dto.Quantity,
                    unit_price = dto.UnitPrice,
                    discount = dto.Discount
                };

                _db.OrderDetails.Add(entity);
                await _db.SaveChangesAsync();

                return entity.order_detail_id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating order detail: " + ex.Message);
            }
        }

        public async Task<bool> UpdateOrderDetailAsync(OrderDetailDTO dto)
        {
            try
            {
                var entity = await _db.OrderDetails.FindAsync(dto.OrderDetailId);
                if (entity == null) return false;

                entity.product_id = dto.ProductId;
                entity.quantity = dto.Quantity;
                entity.unit_price = dto.UnitPrice;
                entity.discount = dto.Discount;

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating order detail: " + ex.Message);
            }
        }

        public async Task<bool> DeleteOrderDetailAsync(int id)
        {
            try
            {
                var entity = await _db.OrderDetails.FindAsync(id);
                if (entity == null) return false;

                _db.OrderDetails.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting order detail: " + ex.Message);
            }
        }


        public async Task<CheckoutResultDTO> CheckoutAsync(CheckoutRequestDTO request)
        {
            // 1. Validate cơ bản
            if (request.Items == null || request.Items.Count == 0)
                return new CheckoutResultDTO { Success = false, Message = "Giỏ hàng trống!" };

            // 2. Chuẩn bị bảng dữ liệu (TVP) để truyền vào SP
            var detailsTable = new DataTable();
            detailsTable.Columns.Add("product_id", typeof(int));
            detailsTable.Columns.Add("quantity", typeof(int));
            detailsTable.Columns.Add("unit_price", typeof(decimal));
            detailsTable.Columns.Add("discount", typeof(decimal));

            foreach (var item in request.Items)
            {
                detailsTable.Rows.Add(item.ProductId, item.Quantity, item.Price, item.Discount);
            }

            // 3. Gọi Stored Procedure
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    // Lấy BranchId và UserId từ Context (Session)
                    parameters.Add("@branch_id", _userContext.CurrentBranchId);
                    parameters.Add("@user_id", _userContext.UserId);

                    parameters.Add("@customer_id", request.CustomerId);
                    parameters.Add("@total_amount", request.TotalAmount);
                    parameters.Add("@points_redeemed", request.PointsUsed);
                    parameters.Add("@points_earned", request.PointsEarned);
                    parameters.Add("@manual_discount", request.ManualDiscount);
                    parameters.Add("@discount_reason", request.ManualDiscountReason);
                    parameters.Add("@parent_order_id", request.ParentOrderId);
                    parameters.Add("@status", request.Status ?? "Completed");
                    parameters.Add("@payment_method", request.PaymentMethod);

                    // Truyền TVP
                    parameters.Add("@details", detailsTable.AsTableValuedParameter("dbo.OrderDetailType"));

                    // Thực thi và nhận kết quả
                    var result = await conn.QuerySingleAsync<dynamic>(
                        "sp_Sales_Checkout",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    if (result != null)
                    {
                        int newOrderId = (int)result.OrderId;
                        string status = request.Status;
                        int branchId = _userContext.CurrentBranchId ?? 1;

                        await _orderRealtimeService.NotifyOrderCreatedAsync(newOrderId, status, branchId, request.ConnectionId);

                        return new CheckoutResultDTO
                        {
                            Success = true,
                            OrderId = newOrderId,
                            Message = result.Message ?? "Thanh toán thành công"
                        };
                    }
                    return new CheckoutResultDTO { Success = false, Message = "Không nhận được phản hồi từ hệ thống." };
                }
                catch (SqlException ex)
                {
                    // Lỗi nghiệp vụ do THROW trong SQL
                    if (ex.Number == 50003)
                    {
                        return new CheckoutResultDTO
                        {
                            Success = false,
                            Message = "Không thể hoàn tiền: Số lượng trả vượt quá số lượng khách đã mua thực tế."
                        };
                    }

                    // Các lỗi SQL khác
                    return new CheckoutResultDTO
                    {
                        Success = false,
                        Message = "Lỗi hệ thống kho bãi. " + ex.Message
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Checkout Error: " + ex.Message);
                    return new CheckoutResultDTO
                    {
                        Success = false,
                        Message = "Có lỗi xảy ra, vui lòng thử lại."
                    };
                }

            }
        }
    }
}
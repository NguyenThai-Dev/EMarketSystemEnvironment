using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EMarket.Events.Class;
using EMarket.Events.Interfaces;
using EMarket.Models;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.InventoryModule.Services.Implementations
{
    public class SupplierDebtAndPaymentService : ISupplierServiceDebtAndPaymentService
    {
        private readonly EMarket_DBEntities _db;
        private readonly ISupplierService _supplierService;
        private readonly ILoginService _loginService;
        private readonly IEventDispatcher _dispatcher;
        private readonly DateTime defaultDate = new DateTime(2000, 1, 1);

        public SupplierDebtAndPaymentService(EMarket_DBEntities db, ISupplierService supplierService, ILoginService loginService, IEventDispatcher eventDispatcher)
        {
            _db = db;
            _supplierService = supplierService;
            _loginService = loginService;
            _dispatcher = eventDispatcher;
        }

        public async Task<List<SupplierDebtDTO>> GetAllSupplierDebtsAsync()
        {
            return await _db.SupplierDebts
                .Select(x => new SupplierDebtDTO
                {
                    DebtId = x.debt_id,
                    PurchaseOrderId = x.purchase_order_id,
                    SupplierId = x.supplier_id,
                    TotalAmount = x.total_amount,
                    PaidAmount = x.paid_amount,
                    UnpaidAmount = x.unpaid_amount,
                    DueDate = x.due_date,
                    Status = x.status,
                    UpdatedAt = x.updated_at ?? defaultDate
                })
                .ToListAsync();
        }

        public async Task<List<SupplierDebtDTO>> GetAllSupplierDebtsAsync(string keyword,
     int? supplierId,
     string status = null,
     DateTime? fromDate = null,
     DateTime? toDate = null)
        {
            // --- 1. Join bảng SupplierDebts với PurchaseOrders ---
            // Chỉ lấy những khoản nợ mà đơn nhập hàng có trạng thái 'Completed'
            var query = from debt in _db.SupplierDebts.AsNoTracking()
                        join po in _db.PurchaseOrders.AsNoTracking() on debt.purchase_order_id equals po.purchase_order_id
                        where po.status == "Completed" // Chỉ lấy đơn đã hoàn tất
                        select debt;

            // --- 2. Filter Logic cơ bản ---
            if (supplierId.HasValue && supplierId.Value > 0)
            {
                query = query.Where(x => x.supplier_id == supplierId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.status == status);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.updated_at >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                // Tối ưu: Lấy đến cuối ngày của toDate
                var endOfDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.updated_at <= endOfDate);
            }

            // --- 3. Xử lý Keyword (Search theo mã PO) ---
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                if (int.TryParse(keyword, out int poId))
                {
                    query = query.Where(x => x.purchase_order_id == poId);
                }
            }

            // --- 4. Lấy dữ liệu về RAM ---
            var rawData = await query
                .OrderByDescending(x => x.updated_at)
                .ToListAsync();

            // --- 5. Resolve tên nhà cung cấp (Batch Fetch) ---
            var supplierIds = rawData.Select(x => x.supplier_id).Distinct().ToList();
            var supplierMap = new Dictionary<int, string>();

            if (supplierIds.Any())
            {
                var suppliers = await _supplierService.GetAllSupplierByIdAsync(supplierIds);
                supplierMap = suppliers.ToDictionary(x => x.SupplierId, x => x.Name);
            }

            // --- 6. Map sang DTO ---
            var resultDTOs = rawData.Select(x => new SupplierDebtDTO
            {
                DebtId = x.debt_id,
                PurchaseOrderId = x.purchase_order_id,
                SupplierName = supplierMap.ContainsKey(x.supplier_id) ? supplierMap[x.supplier_id] : "N/A",
                TotalAmount = x.total_amount,
                PaidAmount = x.paid_amount,
                UnpaidAmount = x.unpaid_amount,
                DueDate = x.due_date,
                CreatedAt = x.updated_at ?? DateTime.Now,
                Status = x.status
            }).ToList();

            // --- 7. Lọc Keyword theo Tên Nhà Cung Cấp trên RAM ---
            if (!string.IsNullOrWhiteSpace(keyword) && !int.TryParse(keyword, out _))
            {
                resultDTOs = resultDTOs.Where(x =>
                    x.SupplierName.ToLower().Contains(keyword.ToLower())
                ).ToList();
            }

            return resultDTOs;
        }



        public async Task<SupplierDebtDTO> GetSupplierDebtByIdAsync(int id)
        {
            var e = await _db.SupplierDebts.FindAsync(id);
            if (e == null) return null;

            return new SupplierDebtDTO
            {
                DebtId = e.debt_id,
                PurchaseOrderId = e.purchase_order_id,
                SupplierId = e.supplier_id,
                TotalAmount = e.total_amount,
                PaidAmount = e.paid_amount,
                UnpaidAmount = e.unpaid_amount,
                DueDate = e.due_date,
                Status = e.status,
                UpdatedAt = e.updated_at ?? defaultDate
            };
        }

        public async Task<SupplierDebtDTO> GetSupplierDebtByPurchaseOrderIdAsync(int purchaseOrderId)
        {
            var e = await _db.SupplierDebts
                .FirstOrDefaultAsync(x => x.purchase_order_id == purchaseOrderId);

            if (e == null) return null;

            return new SupplierDebtDTO
            {
                DebtId = e.debt_id,
                PurchaseOrderId = e.purchase_order_id,
                SupplierId = e.supplier_id,
                TotalAmount = e.total_amount,
                PaidAmount = e.paid_amount,
                UnpaidAmount = e.unpaid_amount,
                DueDate = e.due_date,
                Status = e.status,
                UpdatedAt = e.updated_at ?? defaultDate
            };
        }

        public async Task<bool> CreateSupplierDebtAsync(SupplierDebtDTO dto)
        {
            try
            {
                var e = new SupplierDebt
                {
                    purchase_order_id = dto.PurchaseOrderId,
                    supplier_id = dto.SupplierId,
                    total_amount = dto.TotalAmount,
                    paid_amount = dto.PaidAmount,
                    due_date = dto.DueDate,
                    status = dto.Status,
                    updated_at = DateTime.Now
                };

                _db.SupplierDebts.Add(e);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateSupplierDebtAsync(SupplierDebtDTO dto)
        {
            var e = await _db.SupplierDebts.FindAsync(dto.DebtId);
            if (e == null) return false;

            e.total_amount = dto.TotalAmount;
            e.paid_amount = dto.PaidAmount;
            e.due_date = dto.DueDate;
            e.status = dto.Status;
            e.updated_at = DateTime.Now;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<SupplierPaymentDTO>> GetPaymentsByDebtIdAsync(int debtId)
        {
            return await _db.SupplierPayments
                .Where(x => x.debt_id == debtId)
                .Select(x => new SupplierPaymentDTO
                {
                    PaymentId = x.payment_id,
                    DebtId = x.debt_id,
                    Amount = x.amount,
                    PaymentMethod = x.payment_method,
                    PaymentDate = x.payment_date ?? defaultDate,
                    PaymentProof = x.payment_proof
                })
                .ToListAsync();
        }

        public async Task<bool> CreateSupplierPaymentAsync(SupplierPaymentDTO dto)
        {
            using (var tran = _db.Database.BeginTransaction())
            {
                try
                {
                    var payment = new SupplierPayment
                    {
                        debt_id = dto.DebtId,
                        user_id = _loginService.GetCurrentUserId(),
                        amount = dto.Amount,
                        payment_method = dto.PaymentMethod,
                        payment_proof = dto.PaymentProof,
                        payment_date = DateTime.Now
                    };

                    _db.SupplierPayments.Add(payment);
                    await _db.SaveChangesAsync();

                    await _dispatcher.DispatchAsync(new SupplierPaymentCreatedEvent(payment.payment_id));

                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteSupplierPaymentAsync(int id)
        {
            // 1. Join Payment → Debt (CÙNG MODULE)
            var data = await (
                from p in _db.SupplierPayments
                join d in _db.SupplierDebts on p.debt_id equals d.debt_id
                where p.payment_id == id
                select new
                {
                    Payment = p,
                    SupplierId = d.supplier_id
                }
            ).FirstOrDefaultAsync();

            if (data == null)
                return false;

            // 2. Lấy Supplier qua SERVICE (KHÁC MODULE)
            var supplier = await _supplierService.GetSupplierByIdAsync(data.SupplierId);
            if (supplier == null)
                throw new Exception("Supplier không tồn tại.");

            // 3. Build event
            var deleteEvent = new SupplierPaymentDeletedEvent
            {
                SupplierEmail = supplier.Email,
                SupplierName = supplier.Name,
                Amount = data.Payment.amount ?? -1,
                PaymentId = data.Payment.payment_id
            };

            DeletePaymentProofFile(data.Payment.payment_proof);

            // 4. Xóa payment
            _db.SupplierPayments.Remove(data.Payment);
            await _db.SaveChangesAsync();

            // 5. Dispatch event
            await _dispatcher.DispatchAsync(deleteEvent);

            return true;
        }

        private void DeletePaymentProofFile(string paymentProofPath)
        {
            if (string.IsNullOrWhiteSpace(paymentProofPath))
                return;

            try
            {
                var physicalPath = HttpContext.Current.Server.MapPath(paymentProofPath);

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
            catch
            {
            }
        }


        public async Task<List<SupplierDebtDTO>> GetSupplierDebtsByIdsAsync(List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return new List<SupplierDebtDTO>();

                var result = await _db.SupplierDebts
                    .Where(x => ids.Contains(x.supplier_id))
                    .Select(x => new SupplierDebtDTO
                    {
                        DebtId = x.debt_id,
                        PurchaseOrderId = x.purchase_order_id,
                        SupplierId = x.supplier_id,
                        TotalAmount = x.total_amount,
                        PaidAmount = x.paid_amount,
                        UnpaidAmount = x.unpaid_amount,        // Computed column (PERSISTED)
                        DueDate = x.due_date,
                        Status = x.status,
                        UpdatedAt = x.updated_at ?? defaultDate
                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while loading supplier debts.", ex);
            }
        }

        public async Task<SupplierPaymentDTO> GetPaymentMailInfoAsync(int paymentId)
        {
            var data = await (
                from p in _db.SupplierPayments
                join d in _db.SupplierDebts on p.debt_id equals d.debt_id
                where p.payment_id == paymentId
                select new
                {
                    p,
                    d.supplier_id,
                    d.unpaid_amount
                }
            ).FirstOrDefaultAsync();

            if (data == null) return null;

            var supplier = await _supplierService.GetSupplierByIdAsync(data.supplier_id);

            if (supplier == null) return null;

            return new SupplierPaymentDTO
            {
                PaymentId = data.p.payment_id,
                Amount = data.p.amount,
                PaymentDate = data.p.payment_date ?? DateTime.Now,
                PaymentMethod = data.p.payment_method,
                SupplierName = supplier.Name,
                SupplierEmail = supplier.Email,
                PaymentProof = data.p.payment_proof,
                UnpaidAmountAfterPayment = data.unpaid_amount
            };
        }


        public async Task<List<SupplierDebtDTO>> GetDebtsNearDueDateAsync(int daysBefore)
        {
            var today = DateTime.Today;
            var targetDate = today.AddDays(daysBefore);

            return await _db.SupplierDebts
                .AsNoTracking()
                .Where(x =>
                    x.unpaid_amount > 0 &&
                    x.due_date.HasValue &&
                    x.due_date.Value >= today &&
                    x.due_date.Value <= targetDate
                )
                .Select(x => new SupplierDebtDTO
                {
                    DebtId = x.debt_id,
                    SupplierId = x.supplier_id,
                    PurchaseOrderId = x.purchase_order_id,
                    TotalAmount = x.total_amount,
                    PaidAmount = x.paid_amount,
                    DueDate = x.due_date,
                    Status = x.status
                })
                .ToListAsync();
        }

        public async Task<List<SupplierDebtDTO>> GetOverdueDebtsAsync()
        {
            var today = DateTime.Today;

            return await _db.SupplierDebts
                .AsNoTracking()
                .Where(x =>
                    x.unpaid_amount > 0 &&
                    x.due_date.HasValue &&
                    x.due_date.Value < today
                )
                .Select(x => new SupplierDebtDTO
                {
                    DebtId = x.debt_id,
                    SupplierId = x.supplier_id,
                    PurchaseOrderId = x.purchase_order_id,
                    TotalAmount = x.total_amount,
                    PaidAmount = x.paid_amount,
                    DueDate = x.due_date,
                    Status = x.status
                })
                .ToListAsync();
        }

        public async Task<List<InternalDebtNotificationDTO>> GetInternalDebtDetailAsync(List<int> debtIds)
        {
            // 1. Lấy thông tin người nhận (Role 5) từ Module User trước
            // Chúng ta lấy 1 lần duy nhất thay vì lấy trong vòng lặp
            var accountantEmails = await _loginService.GetEmailsByRoleAsync(5);
            Debug.WriteLine($"[Service] Accountant Emails found: {accountantEmails?.Count ?? 0}");
            if (accountantEmails == null || !accountantEmails.Any()) return new List<InternalDebtNotificationDTO>();

            var recipientEmailString = string.Join(",", accountantEmails);
            var today = DateTime.Now;

            // 2. Truy vấn danh sách các khoản nợ theo danh sách ID truyền vào
            var query = from d in _db.SupplierDebts.AsNoTracking()
                        join po in _db.PurchaseOrders.AsNoTracking() on d.purchase_order_id equals po.purchase_order_id
                        join s in _db.Suppliers.AsNoTracking() on d.supplier_id equals s.supplier_id
                        where debtIds.Contains(d.debt_id) // Lọc theo danh sách ID
                        select new InternalDebtNotificationDTO
                        {
                            DebtId = d.debt_id,
                            PurchaseOrderId = po.purchase_order_id,
                            SupplierName = s.name,
                            UnpaidAmount = d.unpaid_amount,
                            DueDate = d.due_date,
                            RecipientEmail = recipientEmailString,
                            EmployeeName = "Bộ phận Kế toán",
                        };

            var result = await query.ToListAsync();
            Debug.WriteLine($"[Service] Raw Debts in DB: {result.Count}");

            foreach (var item in result)
            {
                if (item.DueDate.HasValue)
                {
                    item.OverdueDays = (int)(today - item.DueDate.Value).TotalDays;
                }
            }

            return result;
        }

    }
}
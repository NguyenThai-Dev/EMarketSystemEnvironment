using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.ExpenseModule.DTOs;
using EMarket.Modules.ExpenseModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.ExpenseModule.Services.Implementations
{
    public class ExpenseService : IExpenseService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly DateTime defaultDate = new DateTime(2000, 1, 1);

        public ExpenseService(
            EMarket_DBEntities db,
            IBranchService branchService,
            IUserService userService)
        {
            _db = db;
            _branchService = branchService;
            _userService = userService;
        }

        public async Task<List<ExpenseDTO>> GetExpensesAsync(
            int? branchId,
            int? categoryId,
            DateTime? fromDate,
            DateTime? toDate,
            string status)
        {
            // JOIN TRONG MODULE: Expense + ExpenseCategory
            var query = _db.Expenses
                .AsNoTracking()
                .Include(x => x.ExpenseCategory)
                .AsQueryable();

            if (branchId.HasValue)
                query = query.Where(x => x.branch_id == branchId.Value);

            if (categoryId.HasValue)
                query = query.Where(x => x.category_id == categoryId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.status == status);

            if (fromDate.HasValue)
                query = query.Where(x => x.expense_date >= fromDate.Value);

            if (toDate.HasValue)
            {
                var to = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.expense_date <= to);
            }

            var expenses = await query
                .OrderByDescending(x => x.expense_date)
                .ToListAsync();

            // KHÁC MODULE → gọi service
            var branchDict = await _branchService.GetBranchDictAsync();
            var userDict = await _userService.GetUserDictAsync();

            return expenses.Select(x => new ExpenseDTO
            {
                ExpenseId = x.expense_id,
                BranchId = x.branch_id,
                CategoryId = x.category_id,
                UserId = x.user_id,

                BranchName = branchDict.TryGetValue(x.branch_id, out var branchDTO)
                    ? branchDTO.Name
                    : "N/A",

                CategoryName = x.ExpenseCategory != null
                    ? x.ExpenseCategory.name
                    : "N/A",

                UserName = userDict.TryGetValue(x.user_id, out var Users)
                    ? Users.FullName
                    : "N/A",

                Amount = x.amount,
                ExpenseDate = x.expense_date ?? DateTime.Now,
                Note = x.note,
                Status = x.status,
                RefImage = x.ref_image,
                ApprovedBy = x.approved_by ?? 0,
                ApproverName = x.approved_by != null && userDict.TryGetValue(x.approved_by.Value, out var Approvers)
                    ? Approvers.FullName
                    : "N/A",
                ApprovedAt = x.approved_at ?? defaultDate,
                RejectedBy = x.rejected_by ?? 0,
                RejectorName = x.rejected_by != null && userDict.TryGetValue(x.rejected_by.Value, out var Rejectors)
                    ? Rejectors.FullName
                    : "N/A",
                RejectedAt = x.rejected_at ?? defaultDate,
                RejectionReason = x.reject_reason,
                PaymentMethod = x.payment_method
            }).ToList();
        }

        public async Task<ExpenseDTO> GetExpenseByIdAsync(int id)
        {
            var x = await _db.Expenses
                .AsNoTracking()
                .Include(e => e.ExpenseCategory)
                .FirstOrDefaultAsync(e => e.expense_id == id);

            if (x == null) return null;

            var branchName = (await _branchService.GetBranchByIdAsync(x.branch_id)).Name;
            var userDict = await _userService.GetUserDictAsync();

            return new ExpenseDTO
            {
                ExpenseId = x.expense_id,
                BranchId = x.branch_id,
                CategoryId = x.category_id,
                UserId = x.user_id,

                BranchName = branchName,
                CategoryName = x.ExpenseCategory?.name ?? "N/A",
                UserName = userDict.TryGetValue(x.user_id, out var Users) ? Users.FullName : "N/A",

                Amount = x.amount,
                ExpenseDate = x.expense_date ?? DateTime.Now,
                Note = x.note,
                Status = x.status,
                RefImage = x.ref_image,
                ApprovedBy = x.approved_by ?? 0,
                ApproverName = x.approved_by != null && userDict.TryGetValue(x.approved_by.Value, out var Approvers)
                    ? Approvers.FullName
                    : "N/A",
                ApprovedAt = x.approved_at ?? defaultDate,
                RejectedBy = x.rejected_by ?? 0,
                RejectorName = x.rejected_by != null && userDict.TryGetValue(x.rejected_by.Value, out var Rejectors)
                    ? Rejectors.FullName
                    : "N/A",
                RejectedAt = x.rejected_at ?? defaultDate,
                RejectionReason = x.reject_reason
            };
        }

        public async Task<bool> CreateExpenseAsync(ExpenseDTO dto)
        {
            try
            {
                var entity = new Expens
                {
                    branch_id = dto.BranchId,
                    category_id = dto.CategoryId,
                    user_id = dto.UserId,
                    amount = dto.Amount,
                    expense_date = dto.ExpenseDate,
                    note = dto.Note,
                    ref_image = dto.RefImage,
                    status = ExpenseStatus.Pending,
                    created_at = DateTime.Now
                };

                _db.Expenses.Add(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task UpdateStatusAsync(
    int expenseId,
    string status,
    int actionUserId,
    string rejectReason = null,
    string paymentMethod = "Cash")
        {
            var item = await _db.Expenses.FindAsync(expenseId);
            if (item == null)
                throw new Exception("Phiếu chi không tồn tại.");

            if (item.status != ExpenseStatus.Pending)
                throw new Exception("Phiếu chi đã được xử lý, không thể thay đổi.");

            if (status == ExpenseStatus.Approved)
            {
                item.status = ExpenseStatus.Approved;
                item.approved_by = actionUserId;
                item.approved_at = DateTime.Now;
                item.payment_method = paymentMethod;

                // clear reject info (defensive)
                item.rejected_by = null;
                item.rejected_at = null;
                item.reject_reason = null;
            }
            else if (status == ExpenseStatus.Rejected)
            {
                if (string.IsNullOrWhiteSpace(rejectReason))
                    throw new Exception("Vui lòng nhập lý do từ chối.");

                item.status = ExpenseStatus.Rejected;
                item.rejected_by = actionUserId;
                item.rejected_at = DateTime.Now;
                item.reject_reason = rejectReason;
            }
            else
            {
                throw new Exception("Trạng thái không hợp lệ.");
            }

            item.updated_at = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var item = await _db.Expenses.FindAsync(id);
            if (item == null) return false;

            if (item.status == ExpenseStatus.Approved)
                throw new InvalidOperationException("Không thể xoá phiếu chi đã được duyệt");

            _db.Expenses.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ExpenseCategoryDTO>> GetActiveExpenseCategoriesAsync()
        {
            return await _db.ExpenseCategories
                .AsNoTracking()
                .Where(c => c.is_active == true)
                .OrderBy(c => c.name)
                .Select(c => new ExpenseCategoryDTO
                {
                    CategoryId = c.category_id,
                    Name = c.name
                })
                .ToListAsync();
        }

        public async Task<List<ExpenseCategoryDTO>> GetAllExpenseCategoriesAsync()
        {
            return await _db.ExpenseCategories
                .AsNoTracking()
                .OrderBy(c => c.name)
                .Select(c => new ExpenseCategoryDTO
                {
                    CategoryId = c.category_id,
                    Name = c.name,
                    Description = c.description,
                    IsActive = c.is_active
                })
                .ToListAsync();
        }

        public async Task<ExpenseCategoryDTO> GetExpenseCategoryByIdAsync(int categoryId)
        {
            var expenseCategory = await _db.ExpenseCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.category_id == categoryId);
            if (expenseCategory == null) return null;
            return new ExpenseCategoryDTO
            {
                CategoryId = expenseCategory.category_id,
                Name = expenseCategory.name,
                Description = expenseCategory.description,
                IsActive = expenseCategory.is_active
            };
        }

        public async Task<bool> CreateExpenseCategoryAsync(ExpenseCategoryDTO dto)
        {
            try
            {
                var entity = new ExpenseCategory
                {
                    name = dto.Name,
                    description = dto.Description,
                    is_active = dto.IsActive ?? false
                };
                _db.ExpenseCategories.Add(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateExpenseCategoryAsync(ExpenseCategoryDTO dto)
        {
            var entity = await _db.ExpenseCategories.FindAsync(dto.CategoryId);
            if (entity == null) return false;
            entity.name = dto.Name;
            entity.description = dto.Description;
            entity.is_active = dto.IsActive ?? false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExpenseCategoryAsync(int categoryId)
        {
            var entity = await _db.ExpenseCategories.FindAsync(categoryId);
            if (entity == null) return false;
            var hasExpenses = await _db.Expenses.AnyAsync(e => e.category_id == categoryId);
            if (hasExpenses)
                throw new InvalidOperationException("Không thể xoá loại chi phí đang có phiếu chi sử dụng.");
            _db.ExpenseCategories.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
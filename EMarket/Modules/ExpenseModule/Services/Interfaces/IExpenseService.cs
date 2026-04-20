using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.ExpenseModule.DTOs;

namespace EMarket.Modules.ExpenseModule.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<List<ExpenseDTO>> GetExpensesAsync(
            int? branchId,
            int? categoryId,
            DateTime? fromDate,
            DateTime? toDate,
            string status);

        Task<ExpenseDTO> GetExpenseByIdAsync(int id);
        Task<bool> CreateExpenseAsync(ExpenseDTO dto);
        Task UpdateStatusAsync(
        int expenseId,
        string status,
        int actionUserId,
        string rejectReason = null,
        string paymentMethod = null
    );
        Task<bool> DeleteExpenseAsync(int id);

        Task<List<ExpenseCategoryDTO>> GetActiveExpenseCategoriesAsync();
        Task<List<ExpenseCategoryDTO>> GetAllExpenseCategoriesAsync();
        Task<ExpenseCategoryDTO> GetExpenseCategoryByIdAsync(int categoryId);
        Task<bool> CreateExpenseCategoryAsync(ExpenseCategoryDTO dto);
        Task<bool> UpdateExpenseCategoryAsync(ExpenseCategoryDTO dto);
        Task<bool> DeleteExpenseCategoryAsync(int categoryId);
    }
}

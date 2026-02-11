using Settlr.Models.Entities;

namespace Settlr.Data.IRepositories;

public interface IExpenseRepository : IGenericRepository<Expense>
{
    Task<IEnumerable<Expense>> GetGroupExpensesAsync(int groupId);
    Task<Expense?> GetExpenseWithSplitsAsync(int expenseId);
    Task<IEnumerable<Expense>> GetUserExpensesAsync(int userId);
}

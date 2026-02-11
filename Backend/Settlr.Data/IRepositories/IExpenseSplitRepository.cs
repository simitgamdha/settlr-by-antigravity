using Settlr.Models.Entities;

namespace Settlr.Data.IRepositories;

public interface IExpenseSplitRepository : IGenericRepository<ExpenseSplit>
{
    Task<IEnumerable<ExpenseSplit>> GetUserSplitsAsync(int userId);
    Task<IEnumerable<ExpenseSplit>> GetExpenseSplitsAsync(int expenseId);
}

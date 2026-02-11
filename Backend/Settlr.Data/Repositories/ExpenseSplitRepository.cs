using Microsoft.EntityFrameworkCore;
using Settlr.Data.IRepositories;
using Settlr.Models.Entities;

namespace Settlr.Data.Repositories;

public class ExpenseSplitRepository : GenericRepository<ExpenseSplit>, IExpenseSplitRepository
{
    public ExpenseSplitRepository(DbContext.ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ExpenseSplit>> GetUserSplitsAsync(int userId)
    {
        return await _dbSet
            .Where(es => es.UserId == userId)
            .Include(es => es.Expense)
                .ThenInclude(e => e.PaidBy)
            .Include(es => es.Expense)
                .ThenInclude(e => e.Group)
            .ToListAsync();
    }

    public async Task<IEnumerable<ExpenseSplit>> GetExpenseSplitsAsync(int expenseId)
    {
        return await _dbSet
            .Where(es => es.ExpenseId == expenseId)
            .Include(es => es.User)
            .ToListAsync();
    }
}

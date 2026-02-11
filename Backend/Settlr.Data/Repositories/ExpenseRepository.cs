using Microsoft.EntityFrameworkCore;
using Settlr.Data.IRepositories;
using Settlr.Models.Entities;

namespace Settlr.Data.Repositories;

public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
{
    public ExpenseRepository(DbContext.ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Expense>> GetGroupExpensesAsync(int groupId)
    {
        return await _dbSet
            .Where(e => e.GroupId == groupId)
            .Include(e => e.PaidBy)
            .Include(e => e.Group)
            .Include(e => e.Splits)
                .ThenInclude(s => s.User)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Expense?> GetExpenseWithSplitsAsync(int expenseId)
    {
        return await _dbSet
            .Include(e => e.PaidBy)
            .Include(e => e.Group)
            .Include(e => e.Splits)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(e => e.Id == expenseId);
    }

    public async Task<IEnumerable<Expense>> GetUserExpensesAsync(int userId)
    {
        return await _dbSet
            .Where(e => e.Splits.Any(s => s.UserId == userId) || e.PaidById == userId)
            .Include(e => e.PaidBy)
            .Include(e => e.Group)
            .Include(e => e.Splits)
                .ThenInclude(s => s.User)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
}

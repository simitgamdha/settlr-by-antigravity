namespace Settlr.Models.Entities;

public class ExpenseSplit
{
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public int UserId { get; set; }
    public decimal ShareAmount { get; set; }

    // Navigation properties
    public Expense Expense { get; set; } = null!;
    public User User { get; set; } = null!;
}

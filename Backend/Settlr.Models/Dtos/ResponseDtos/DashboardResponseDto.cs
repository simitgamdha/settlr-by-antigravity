namespace Settlr.Models.Dtos.ResponseDtos;

public class DashboardResponseDto
{
    public decimal TotalOwed { get; set; } // Total amount user owes to others
    public decimal TotalOwedTo { get; set; } // Total amount owed to user
    public List<ExpenseResponseDto> RecentExpenses { get; set; } = new List<ExpenseResponseDto>();
    public List<GroupResponseDto> Groups { get; set; } = new List<GroupResponseDto>();
}

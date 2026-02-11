using Moq;
using Settlr.Data.IRepositories;
using Settlr.Models.Entities;
using Settlr.Services.Services;
using Xunit;

namespace Settlr.Tests;

public class DashboardServiceTests
{
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly DashboardService _dashboardService;

    public DashboardServiceTests()
    {
        _expenseRepositoryMock = new Mock<IExpenseRepository>();
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _dashboardService = new DashboardService(
            _expenseRepositoryMock.Object,
            _groupRepositoryMock.Object);
    }

    [Fact]
    public async Task GetDashboardDataAsync_ShouldCalculateTotalsCorrectly()
    {
        // Arrange
        int userId = 1;
        var user1 = new User { Id = 1, Name = "User 1" };
        var user2 = new User { Id = 2, Name = "User 2" };

        var expenses = new List<Expense>
        {
            // User 1 paid 100, split 50/50 with User 2. User 1 is owed 50.
            new Expense
            {
                Id = 1,
                Amount = 100,
                PaidById = 1,
                PaidBy = user1,
                CreatedAt = DateTime.UtcNow,
                Group = new Group { Name = "G1" },
                Splits = new List<ExpenseSplit>
                {
                    new ExpenseSplit { UserId = 1, User = user1, ShareAmount = 50 },
                    new ExpenseSplit { UserId = 2, User = user2, ShareAmount = 50 }
                }
            },
            // User 2 paid 200, split 100/100 with User 1. User 1 owes 100.
            new Expense
            {
                Id = 2,
                Amount = 200,
                PaidById = 2,
                PaidBy = user2,
                CreatedAt = DateTime.UtcNow,
                Group = new Group { Name = "G1" },
                Splits = new List<ExpenseSplit>
                {
                    new ExpenseSplit { UserId = 1, User = user1, ShareAmount = 100 },
                    new ExpenseSplit { UserId = 2, User = user2, ShareAmount = 100 }
                }
            }
        };

        _expenseRepositoryMock.Setup(repo => repo.GetUserExpensesAsync(userId))
            .ReturnsAsync(expenses);
        _groupRepositoryMock.Setup(repo => repo.GetUserGroupsAsync(userId))
            .ReturnsAsync(new List<Group>());

        // Act
        var result = await _dashboardService.GetDashboardDataAsync(userId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(50, result.Data.TotalOwedTo); // Owed 50 from first expense
        Assert.Equal(100, result.Data.TotalOwed);  // Owes 100 from second expense
    }
}

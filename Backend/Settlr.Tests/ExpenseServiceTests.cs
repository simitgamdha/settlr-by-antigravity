using Moq;
using Settlr.Common.Messages;
using Settlr.Data.IRepositories;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Entities;
using Settlr.Services.Services;
using Xunit;

namespace Settlr.Tests;

public class ExpenseServiceTests
{
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
    private readonly Mock<IExpenseSplitRepository> _expenseSplitRepositoryMock;
    private readonly Mock<IGroupMemberRepository> _groupMemberRepositoryMock;
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly ExpenseService _expenseService;

    public ExpenseServiceTests()
    {
        _expenseRepositoryMock = new Mock<IExpenseRepository>();
        _expenseSplitRepositoryMock = new Mock<IExpenseSplitRepository>();
        _groupMemberRepositoryMock = new Mock<IGroupMemberRepository>();
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _expenseService = new ExpenseService(
            _expenseRepositoryMock.Object,
            _expenseSplitRepositoryMock.Object,
            _groupMemberRepositoryMock.Object,
            _groupRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateExpenseAsync_ShouldReturnSuccess_AndSplitCorrectly()
    {
        // Arrange
        int userId = 1;
        var request = new CreateExpenseRequestDto
        {
            GroupId = 1,
            Amount = 300,
            Description = "Food"
        };

        var members = new List<GroupMember>
        {
            new GroupMember { UserId = 1, User = new User { Name = "User 1" } },
            new GroupMember { UserId = 2, User = new User { Name = "User 2" } },
            new GroupMember { UserId = 3, User = new User { Name = "User 3" } }
        };

        _groupMemberRepositoryMock.Setup(repo => repo.IsUserMemberOfGroupAsync(userId, request.GroupId))
            .ReturnsAsync(true);
        _groupMemberRepositoryMock.Setup(repo => repo.GetGroupMembersAsync(request.GroupId))
            .ReturnsAsync(members);

        var savedExpense = new Expense
        {
            Id = 1,
            GroupId = request.GroupId,
            Amount = request.Amount,
            Description = request.Description,
            PaidById = userId,
            PaidBy = new User { Name = "User 1" },
            Group = new Group { Name = "Test Group" }
        };
        savedExpense.Splits = members.Select(m => new ExpenseSplit
        {
            UserId = m.UserId,
            User = m.User,
            ShareAmount = 100
        }).ToList();

        _expenseRepositoryMock.Setup(repo => repo.GetExpenseWithSplitsAsync(It.IsAny<int>()))
            .ReturnsAsync(savedExpense);

        // Act
        var result = await _expenseService.CreateExpenseAsync(userId, request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(3, result.Data.Splits.Count);
        Assert.All(result.Data.Splits, s => Assert.Equal(100, s.ShareAmount));
        _expenseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Expense>()), Times.Once);
        _expenseSplitRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ExpenseSplit>()), Times.Exactly(3));
    }

    [Fact]
    public async Task CreateExpenseAsync_ShouldReturnFail_WhenUserNotMember()
    {
        // Arrange
        _groupMemberRepositoryMock.Setup(repo => repo.IsUserMemberOfGroupAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        // Act
        var result = await _expenseService.CreateExpenseAsync(1, new CreateExpenseRequestDto { GroupId = 1 });

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(Messages.UserNotMemberOfGroup, result.Message);
    }

    [Fact]
    public async Task GetGroupBalancesAsync_ShouldCalculateCorrectly()
    {
        // Arrange
        int groupId = 1;
        var user1 = new User { Id = 1, Name = "User 1" };
        var user2 = new User { Id = 2, Name = "User 2" };

        var expenses = new List<Expense>
        {
            new Expense
            {
                Id = 1,
                Amount = 100,
                PaidById = 1,
                PaidBy = user1,
                Splits = new List<ExpenseSplit>
                {
                    new ExpenseSplit { UserId = 1, User = user1, ShareAmount = 50 },
                    new ExpenseSplit { UserId = 2, User = user2, ShareAmount = 50 }
                }
            }
        };

        _groupMemberRepositoryMock.Setup(repo => repo.IsUserMemberOfGroupAsync(1, groupId))
            .ReturnsAsync(true);
        _expenseRepositoryMock.Setup(repo => repo.GetGroupExpensesAsync(groupId))
            .ReturnsAsync(expenses);

        // Act
        var result = await _expenseService.GetGroupBalancesAsync(1, groupId);

        // Assert
        Assert.True(result.Succeeded);
        var balance1 = result.Data.First(b => b.UserId == 1).Balance; // 100 paid - 50 share = +50
        var balance2 = result.Data.First(b => b.UserId == 2).Balance; // 0 paid - 50 share = -50
        Assert.Equal(50, balance1);
        Assert.Equal(-50, balance2);
    }
}

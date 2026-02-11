using Settlr.Common.Response;
using Settlr.Data.IRepositories;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Models.Entities;
using Settlr.Services.IServices;

namespace Settlr.Services.Services;

/// <summary>
/// Service responsible for gathering and calculating dashboard statistics.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IGroupRepository _groupRepository;

    public DashboardService(
        IExpenseRepository expenseRepository,
        IGroupRepository groupRepository)
    {
        _expenseRepository = expenseRepository;
        _groupRepository = groupRepository;
    }

    /// <summary>
    /// Retrieves total balance (owed/owed to) and recent activity for a specific user.
    /// </summary>
    public async Task<Response<DashboardResponseDto>> GetDashboardDataAsync(int userId)
    {
        // Fetch all expenses involving the user to calculate net balances
        IEnumerable<Expense> userExpenses = await _expenseRepository.GetUserExpensesAsync(userId);
        List<Expense> expensesList = userExpenses.ToList();

        decimal totalOwed = 0;
        decimal totalOwedTo = 0;

        foreach (Expense expense in expensesList)
        {
            // Scenario 1: User is the one who paid
            if (expense.PaidById == userId)
            {
                // User is owed the total amount minus their own share
                ExpenseSplit? userSplit = expense.Splits.FirstOrDefault(s => s.UserId == userId);
                decimal userShare = userSplit?.ShareAmount ?? 0;
                totalOwedTo += expense.Amount - userShare;
            }
            // Scenario 2: Someone else paid, user is a participant
            else
            {
                // User owes their share to the payer
                ExpenseSplit? userSplit = expense.Splits.FirstOrDefault(s => s.UserId == userId);
                if (userSplit != null)
                {
                    totalOwed += userSplit.ShareAmount;
                }
            }
        }

        // Extract the 10 most recent expenses for the "Recent Activity" feed
        List<ExpenseResponseDto> recentExpenses = expensesList
            .OrderByDescending(e => e.CreatedAt)
            .Take(10)
            .Select(MapToExpenseResponseDto)
            .ToList();

        // Fetch groups the user is part of to show in the dashboard sidebar/list
        IEnumerable<Group> userGroups = await _groupRepository.GetUserGroupsAsync(userId);
        List<GroupResponseDto> groups = userGroups.Select(MapToGroupResponseDto).ToList();

        DashboardResponseDto response = new DashboardResponseDto
        {
            TotalOwed = totalOwed,
            TotalOwedTo = totalOwedTo,
            RecentExpenses = recentExpenses,
            Groups = groups
        };

        return Response<DashboardResponseDto>.Success(response);
    }

    /// <summary>
    /// Helper to map internal Expense entity to a clean API response DTO.
    /// </summary>
    private ExpenseResponseDto MapToExpenseResponseDto(Expense expense)
    {
        return new ExpenseResponseDto
        {
            Id = expense.Id,
            GroupId = expense.GroupId,
            GroupName = expense.Group.Name,
            Amount = expense.Amount,
            Description = expense.Description,
            PaidById = expense.PaidById,
            PaidByName = expense.PaidBy.Name,
            CreatedAt = expense.CreatedAt,
            Splits = expense.Splits.Select(s => new ExpenseSplitResponseDto
            {
                UserId = s.UserId,
                UserName = s.User.Name,
                ShareAmount = s.ShareAmount
            }).ToList()
        };
    }

    /// <summary>
    /// Helper to map internal Group entity to a clean API response DTO.
    /// </summary>
    private GroupResponseDto MapToGroupResponseDto(Group group)
    {
        return new GroupResponseDto
        {
            Id = group.Id,
            Name = group.Name,
            CreatedById = group.CreatedById,
            CreatedByName = group.CreatedBy.Name,
            CreatedAt = group.CreatedAt,
            Members = group.Members.Select(m => new GroupMemberResponseDto
            {
                UserId = m.UserId,
                UserName = m.User.Name,
                UserEmail = m.User.Email,
                JoinedAt = m.JoinedAt
            }).ToList()
        };
    }
}

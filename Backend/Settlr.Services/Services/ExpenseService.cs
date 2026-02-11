using Settlr.Common.Messages;
using Settlr.Common.Response;
using Settlr.Data.IRepositories;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Models.Entities;
using Settlr.Services.IServices;
using Microsoft.AspNetCore.SignalR;
using Settlr.Web.Hubs;

namespace Settlr.Services.Services;

/// <summary>
/// Service handling all expense-related operations including creation and balance calculation within groups.
/// </summary>
public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseSplitRepository _expenseSplitRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IHubContext<SettlrHub> _hubContext;

    public ExpenseService(
        IExpenseRepository expenseRepository,
        IExpenseSplitRepository expenseSplitRepository,
        IGroupMemberRepository groupMemberRepository,
        IGroupRepository groupRepository,
        IHubContext<SettlrHub> hubContext)
    {
        _expenseRepository = expenseRepository;
        _expenseSplitRepository = expenseSplitRepository;
        _groupMemberRepository = groupMemberRepository;
        _groupRepository = groupRepository;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Creates a new expense and automatically splits it equally among all group members.
    /// Handles rounding remainders by adding them to the first participant's share.
    /// </summary>
    public async Task<Response<ExpenseResponseDto>> CreateExpenseAsync(int userId, CreateExpenseRequestDto request)
    {
        // 1. Authorization check: Is the person adding the expense actually in the group?
        if (!await _groupMemberRepository.IsUserMemberOfGroupAsync(userId, request.GroupId))
        {
            return Response<ExpenseResponseDto>.Fail(Messages.UserNotMemberOfGroup, 403);
        }

        // 2. Fetch all members to determine the split denominator
        IEnumerable<GroupMember> members = await _groupMemberRepository.GetGroupMembersAsync(request.GroupId);
        List<GroupMember> membersList = members.ToList();

        if (membersList.Count == 0)
        {
            return Response<ExpenseResponseDto>.Fail(Messages.GroupNotFound, 404);
        }

        // 3. Persist the core expense record
        Expense expense = new Expense
        {
            GroupId = request.GroupId,
            Amount = request.Amount,
            Description = request.Description,
            PaidById = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _expenseRepository.AddAsync(expense);
        await _expenseRepository.SaveChangesAsync();

        // 4. Split Calculation Algorithm
        // We split the amount equally. To avoid losing pennies due to division, 
        // we calculate the base share and keep track of the remainder.
        decimal shareAmount = Math.Round(request.Amount / membersList.Count, 2);
        decimal remainder = request.Amount - (shareAmount * membersList.Count);

        // 5. Generate individual split records
        for (int i = 0; i < membersList.Count; i++)
        {
            decimal splitAmount = shareAmount;
            
            // The first person takes the rounding remainder (e.g., if 10.00 is split 3 ways, 
            // person 1 gets 3.34, others get 3.33).
            if (i == 0)
            {
                splitAmount += remainder;
            }

            ExpenseSplit split = new ExpenseSplit
            {
                ExpenseId = expense.Id,
                UserId = membersList[i].UserId,
                ShareAmount = splitAmount
            };

            await _expenseSplitRepository.AddAsync(split);
        }

        await _expenseSplitRepository.SaveChangesAsync();

        // Reload data to include populated navigation properties for the response
        Expense? expenseWithSplits = await _expenseRepository.GetExpenseWithSplitsAsync(expense.Id);
        ExpenseResponseDto response = MapToExpenseResponseDto(expenseWithSplits!);

        return Response<ExpenseResponseDto>.Success(response, Messages.ExpenseCreatedSuccessfully);
        
        // SignalR: Notify all clients in this group that a new expense was added
        await _hubContext.Clients.Group(request.GroupId.ToString()).SendAsync("ReceiveExpenseUpdate", response);
    }

    /// <summary>
    /// Returns a full list of expenses for a specific group.
    /// </summary>
    public async Task<Response<List<ExpenseResponseDto>>> GetGroupExpensesAsync(int userId, int groupId)
    {
        if (!await _groupMemberRepository.IsUserMemberOfGroupAsync(userId, groupId))
        {
            return Response<List<ExpenseResponseDto>>.Fail(Messages.UserNotMemberOfGroup, 403);
        }

        IEnumerable<Expense> expenses = await _expenseRepository.GetGroupExpensesAsync(groupId);
        List<ExpenseResponseDto> response = expenses.Select(MapToExpenseResponseDto).ToList();

        return Response<List<ExpenseResponseDto>>.Success(response);
    }

    /// <summary>
    /// Calculates net balances for every user in a group based on all historic expenses.
    /// This is used to show who owes who how much.
    /// </summary>
    public async Task<Response<List<BalanceResponseDto>>> GetGroupBalancesAsync(int userId, int groupId)
    {
        if (!await _groupMemberRepository.IsUserMemberOfGroupAsync(userId, groupId))
        {
            return Response<List<BalanceResponseDto>>.Fail(Messages.UserNotMemberOfGroup, 403);
        }

        IEnumerable<Expense> expenses = await _expenseRepository.GetGroupExpensesAsync(groupId);

        // Track net flow: positive = person is owed money, negative = person owes money
        Dictionary<int, decimal> balances = new Dictionary<int, decimal>();
        Dictionary<int, string> userNames = new Dictionary<int, string>();

        foreach (Expense expense in expenses)
        {
            if (!balances.ContainsKey(expense.PaidById))
            {
                balances[expense.PaidById] = 0;
                userNames[expense.PaidById] = expense.PaidBy.Name;
            }

            // Payer is credited the full amount (they spent it)
            balances[expense.PaidById] += expense.Amount;

            // Every participant (including the payer) is debited their share
            foreach (ExpenseSplit split in expense.Splits)
            {
                if (!balances.ContainsKey(split.UserId))
                {
                    balances[split.UserId] = 0;
                    userNames[split.UserId] = split.User.Name;
                }

                balances[split.UserId] -= split.ShareAmount;
            }
        }

        List<BalanceResponseDto> response = balances.Select(kvp => new BalanceResponseDto
        {
            UserId = kvp.Key,
            UserName = userNames[kvp.Key],
            Balance = kvp.Value
        }).ToList();

        return Response<List<BalanceResponseDto>>.Success(response);
    }

    /// <summary>
    /// Standard mapping from Entity to Response DTO.
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
}

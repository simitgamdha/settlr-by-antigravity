using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;

namespace Settlr.Services.IServices;

public interface IExpenseService
{
    Task<Response<ExpenseResponseDto>> CreateExpenseAsync(int userId, CreateExpenseRequestDto request);
    Task<Response<List<ExpenseResponseDto>>> GetGroupExpensesAsync(int userId, int groupId);
    Task<Response<List<BalanceResponseDto>>> GetGroupBalancesAsync(int userId, int groupId);
}

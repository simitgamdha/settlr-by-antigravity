using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;

namespace Settlr.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private int GetUserId()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpPost]
    public async Task<ActionResult<Response<ExpenseResponseDto>>> CreateExpense([FromBody] CreateExpenseRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int userId = GetUserId();
        Response<ExpenseResponseDto> response = await _expenseService.CreateExpenseAsync(userId, request);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }

    [HttpGet("group/{groupId}")]
    public async Task<ActionResult<Response<List<ExpenseResponseDto>>>> GetGroupExpenses(int groupId)
    {
        int userId = GetUserId();
        Response<List<ExpenseResponseDto>> response = await _expenseService.GetGroupExpensesAsync(userId, groupId);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }

    [HttpGet("group/{groupId}/balances")]
    public async Task<ActionResult<Response<List<BalanceResponseDto>>>> GetGroupBalances(int groupId)
    {
        int userId = GetUserId();
        Response<List<BalanceResponseDto>> response = await _expenseService.GetGroupBalancesAsync(userId, groupId);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }
}

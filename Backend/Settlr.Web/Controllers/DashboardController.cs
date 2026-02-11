using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;

namespace Settlr.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    private int GetUserId()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<Response<DashboardResponseDto>>> GetDashboard()
    {
        int userId = GetUserId();
        Response<DashboardResponseDto> response = await _dashboardService.GetDashboardDataAsync(userId);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }
}

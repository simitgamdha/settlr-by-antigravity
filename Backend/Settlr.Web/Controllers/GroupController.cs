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
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    private int GetUserId()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpPost]
    public async Task<ActionResult<Response<GroupResponseDto>>> CreateGroup([FromBody] CreateGroupRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int userId = GetUserId();
        Response<GroupResponseDto> response = await _groupService.CreateGroupAsync(userId, request);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }

    [HttpPost("members")]
    public async Task<ActionResult<Response<GroupResponseDto>>> AddMember([FromBody] AddGroupMemberRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int userId = GetUserId();
        Response<GroupResponseDto> response = await _groupService.AddMemberAsync(userId, request);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<Response<List<GroupResponseDto>>>> GetUserGroups()
    {
        int userId = GetUserId();
        Response<List<GroupResponseDto>> response = await _groupService.GetUserGroupsAsync(userId);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }

    [HttpGet("{groupId}")]
    public async Task<ActionResult<Response<GroupResponseDto>>> GetGroupById(int groupId)
    {
        int userId = GetUserId();
        Response<GroupResponseDto> response = await _groupService.GetGroupByIdAsync(userId, groupId);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }
}

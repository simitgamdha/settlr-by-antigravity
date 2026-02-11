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
/// Service managing group-related features: creation, viewing user groups, and adding new members.
/// </summary>
public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHubContext<SettlrHub> _hubContext;

    public GroupService(
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository,
        IUserRepository userRepository,
        IHubContext<SettlrHub> hubContext)
    {
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
        _userRepository = userRepository;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Creates a new group and automatically joins the creator as the first member.
    /// </summary>
    public async Task<Response<GroupResponseDto>> CreateGroupAsync(int userId, CreateGroupRequestDto request)
    {
        // 1. Persist the group entity
        Group group = new Group
        {
            Name = request.Name,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _groupRepository.AddAsync(group);
        await _groupRepository.SaveChangesAsync();

        // 2. The person who creates the group is automatically its first member
        GroupMember member = new GroupMember
        {
            GroupId = group.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        };

        await _groupMemberRepository.AddAsync(member);
        await _groupMemberRepository.SaveChangesAsync();

        // Reload group with its Member collection populated to return a rich DTO
        Group? groupWithMembers = await _groupRepository.GetGroupWithMembersAsync(group.Id);
        GroupResponseDto response = MapToGroupResponseDto(groupWithMembers!);

        return Response<GroupResponseDto>.Success(response, Messages.GroupCreatedSuccessfully);
    }

    /// <summary>
    /// Adds a new user to a group by their email address.
    /// Requires the inviter (userId) to be an existing member of the group.
    /// </summary>
    public async Task<Response<GroupResponseDto>> AddMemberAsync(int userId, AddGroupMemberRequestDto request)
    {
        // Security check: Verify the group exists and the requester belongs to it
        Group? group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId);
        if (group == null)
        {
            return Response<GroupResponseDto>.Fail(Messages.GroupNotFound, 404);
        }

        if (!await _groupMemberRepository.IsUserMemberOfGroupAsync(userId, request.GroupId))
        {
            return Response<GroupResponseDto>.Fail(Messages.UserNotMemberOfGroup, 403);
        }

        // Find the target user in the database by email
        User? userToAdd = await _userRepository.GetByEmailAsync(request.UserEmail);
        if (userToAdd == null)
        {
            return Response<GroupResponseDto>.Fail(Messages.UserNotFound, 404);
        }

        // Prevent duplicate memberships
        if (await _groupMemberRepository.IsUserMemberOfGroupAsync(userToAdd.Id, request.GroupId))
        {
            return Response<GroupResponseDto>.Fail(Messages.MemberAlreadyExists, 400);
        }

        // Join the user to the group
        GroupMember member = new GroupMember
        {
            GroupId = request.GroupId,
            UserId = userToAdd.Id,
            JoinedAt = DateTime.UtcNow
        };

        await _groupMemberRepository.AddAsync(member);
        await _groupMemberRepository.SaveChangesAsync();

        Group? updatedGroup = await _groupRepository.GetGroupWithMembersAsync(request.GroupId);
        GroupResponseDto response = MapToGroupResponseDto(updatedGroup!);

        return Response<GroupResponseDto>.Success(response, Messages.MemberAddedSuccessfully);
        
        // SignalR: Notify all clients in this group that the member list has changed
        await _hubContext.Clients.Group(request.GroupId.ToString()).SendAsync("ReceiveGroupUpdate", response);
    }

    /// <summary>
    /// Fetches all groups where the specified user is a member.
    /// </summary>
    public async Task<Response<List<GroupResponseDto>>> GetUserGroupsAsync(int userId)
    {
        IEnumerable<Group> groups = await _groupRepository.GetUserGroupsAsync(userId);
        List<GroupResponseDto> response = groups.Select(MapToGroupResponseDto).ToList();

        return Response<List<GroupResponseDto>>.Success(response);
    }

    /// <summary>
    /// Retrieves full details for a single group, including all members.
    /// </summary>
    public async Task<Response<GroupResponseDto>> GetGroupByIdAsync(int userId, int groupId)
    {
        if (!await _groupMemberRepository.IsUserMemberOfGroupAsync(userId, groupId))
        {
            return Response<GroupResponseDto>.Fail(Messages.UserNotMemberOfGroup, 403);
        }

        Group? group = await _groupRepository.GetGroupWithMembersAsync(groupId);
        if (group == null)
        {
            return Response<GroupResponseDto>.Fail(Messages.GroupNotFound, 404);
        }

        GroupResponseDto response = MapToGroupResponseDto(group);

        return Response<GroupResponseDto>.Success(response);
    }

    /// <summary>
    /// Maps internal Group entity to clean Response DTO.
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

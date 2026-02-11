using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;

namespace Settlr.Services.IServices;

public interface IGroupService
{
    Task<Response<GroupResponseDto>> CreateGroupAsync(int userId, CreateGroupRequestDto request);
    Task<Response<GroupResponseDto>> AddMemberAsync(int userId, AddGroupMemberRequestDto request);
    Task<Response<List<GroupResponseDto>>> GetUserGroupsAsync(int userId);
    Task<Response<GroupResponseDto>> GetGroupByIdAsync(int userId, int groupId);
}

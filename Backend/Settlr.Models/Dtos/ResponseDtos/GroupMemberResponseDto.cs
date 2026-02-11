namespace Settlr.Models.Dtos.ResponseDtos;

public class GroupMemberResponseDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

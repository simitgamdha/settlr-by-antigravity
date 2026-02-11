namespace Settlr.Models.Dtos.ResponseDtos;

public class GroupResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<GroupMemberResponseDto> Members { get; set; } = new List<GroupMemberResponseDto>();
}

using System.ComponentModel.DataAnnotations;

namespace Settlr.Models.Dtos.RequestDtos;

public class AddGroupMemberRequestDto
{
    [Required(ErrorMessage = "Group ID is required")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "User email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string UserEmail { get; set; } = string.Empty;
}

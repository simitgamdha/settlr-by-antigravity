using System.ComponentModel.DataAnnotations;

namespace Settlr.Models.Dtos.RequestDtos;

public class CreateGroupRequestDto
{
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Group name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
}

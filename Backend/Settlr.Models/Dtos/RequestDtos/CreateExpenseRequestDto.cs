using System.ComponentModel.DataAnnotations;

namespace Settlr.Models.Dtos.RequestDtos;

public class CreateExpenseRequestDto
{
    [Required(ErrorMessage = "Group ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid group ID")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be between 0.01 and 999,999.99")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 500 characters")]
    public string Description { get; set; } = string.Empty;
}

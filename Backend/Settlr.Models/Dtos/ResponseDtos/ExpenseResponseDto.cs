namespace Settlr.Models.Dtos.ResponseDtos;

public class ExpenseResponseDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int PaidById { get; set; }
    public string PaidByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ExpenseSplitResponseDto> Splits { get; set; } = new List<ExpenseSplitResponseDto>();
}

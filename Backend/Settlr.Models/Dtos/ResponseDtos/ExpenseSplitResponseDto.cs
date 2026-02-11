namespace Settlr.Models.Dtos.ResponseDtos;

public class ExpenseSplitResponseDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal ShareAmount { get; set; }
}

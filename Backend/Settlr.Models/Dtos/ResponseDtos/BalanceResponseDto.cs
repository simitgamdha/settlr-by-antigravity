namespace Settlr.Models.Dtos.ResponseDtos;

public class BalanceResponseDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Balance { get; set; } // Positive = owed to user, Negative = user owes
}

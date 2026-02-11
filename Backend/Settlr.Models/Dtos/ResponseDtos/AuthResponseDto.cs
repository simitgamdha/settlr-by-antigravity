namespace Settlr.Models.Dtos.ResponseDtos;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserResponseDto User { get; set; } = null!;
}

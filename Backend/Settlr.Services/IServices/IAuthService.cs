using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;

namespace Settlr.Services.IServices;

public interface IAuthService
{
    Task<Response<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<Response<AuthResponseDto>> LoginAsync(LoginRequestDto request);
}

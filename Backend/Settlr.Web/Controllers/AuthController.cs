using Microsoft.AspNetCore.Mvc;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;

namespace Settlr.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Response<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Response<AuthResponseDto> response = await _authService.RegisterAsync(request);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<Response<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Response<AuthResponseDto> response = await _authService.LoginAsync(request);

        if (!response.Succeeded)
        {
            return StatusCode(response.StatusCode, response);
        }

        return Ok(response);
    }
}

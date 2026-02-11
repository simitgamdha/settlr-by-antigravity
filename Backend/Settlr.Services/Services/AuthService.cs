using Settlr.Common.Helper;
using Settlr.Common.Messages;
using Settlr.Common.Response;
using Settlr.Data.IRepositories;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Models.Entities;
using Settlr.Services.IServices;

namespace Settlr.Services.Services;

/// <summary>
/// Service handling user authentication: Registration (sign up) and Login.
/// Uses BCrypt for secure password hashing and JwtHelper for token generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtHelper _jwtHelper;

    public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
    {
        _userRepository = userRepository;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// Registers a new user. Hashes the password and returns a JWT token immediately.
    /// </summary>
    public async Task<Response<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        // 1. Uniqueness Check: Every account must have a unique email address
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return Response<AuthResponseDto>.Fail(Messages.UserAlreadyExists, 400);
        }

        // 2. Security: Never store plain-text passwords. BCrypt handles the salt and hash.
        User user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        // 3. Convenience: Generate a token so the user is logged in immediately after signup
        string token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Name);

        AuthResponseDto response = new AuthResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };

        return Response<AuthResponseDto>.Success(response, Messages.RegistrationSuccessful);
    }

    /// <summary>
    /// Validates user credentials and returns a fresh JWT token on success.
    /// </summary>
    public async Task<Response<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        // 1. Identify: Find user record by the provided email
        User? user = await _userRepository.GetByEmailAsync(request.Email);
        
        // 2. Verify: Check if user exists AND the password hash matches the attempt
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Response<AuthResponseDto>.Fail(Messages.InvalidCredentials, 401);
        }

        // 3. Standard JWT response for stateful frontend experience
        string token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Name);

        AuthResponseDto response = new AuthResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };

        return Response<AuthResponseDto>.Success(response, Messages.LoginSuccessful);
    }
}

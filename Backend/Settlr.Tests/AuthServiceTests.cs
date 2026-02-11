using Moq;
using Settlr.Common.Helper;
using Settlr.Common.Messages;
using Settlr.Data.IRepositories;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Entities;
using Settlr.Services.Services;
using Xunit;

namespace Settlr.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly JwtHelper _jwtHelper;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtHelper = new JwtHelper("super_secret_key_for_testing_purposes_123", "test_issuer", "test_audience", 60);
        _authService = new AuthService(_userRepositoryMock.Object, _jwtHelper);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123"
        };

        _userRepositoryMock.Setup(repo => repo.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(Messages.RegistrationSuccessful, result.Message);
        Assert.NotNull(result.Data.Token);
        Assert.Equal(request.Email, result.Data.User.Email);
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFail_WhenUserAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123"
        };

        _userRepositoryMock.Setup(repo => repo.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(Messages.UserAlreadyExists, result.Message);
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "password123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Name = "Test User",
            PasswordHash = passwordHash
        };

        var request = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = password
        };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(Messages.LoginSuccessful, result.Message);
        Assert.NotNull(result.Data.Token);
        Assert.Equal(user.Id, result.Data.User.Id);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(Messages.InvalidCredentials, result.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFail_WhenPasswordIsIncorrect()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correct_password");
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Name = "Test User",
            PasswordHash = passwordHash
        };

        var request = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "wrong_password"
        };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(Messages.InvalidCredentials, result.Message);
    }
}

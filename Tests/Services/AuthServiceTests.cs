using System.Security.Cryptography;
using System.Text;
using Core.DTOS.Auth;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Service;

namespace Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _sut; // system under test

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();
        
        // Setup configuration for JWT
        _configurationMock.Setup(x => x["JWT:SecretKey"])
            .Returns("Jz7x9fPq2rT4vW6yB8uD0gH1jK3lM5nO7pQ9sS1vU3wX5yZ7aB8cD0eF2gH4jK6l");

        _sut = new AuthService(_configurationMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsBusinessRuleException()
    {
        // Arrange
        var registerDto = new RegisterDto("testuser", "test@test.com", "password123");
        _userRepositoryMock.Setup(x => x.EmailExistsAsync(registerDto.Email))
            .ReturnsAsync(true);

        // Act
        var act = () => _sut.RegisterAsync(registerDto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Email already exists");
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsAuthResponse()
    {
        // Arrange
        var registerDto = new RegisterDto("testuser", "test@test.com", "password123");
        _userRepositoryMock.Setup(x => x.EmailExistsAsync(registerDto.Email))
            .ReturnsAsync(false);

        User? savedUser = null;
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Callback<User>(user => savedUser = user)
            .ReturnsAsync((User user) => user);

        // Act
        var result = await _sut.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(registerDto.Email);
        result.Username.Should().Be(registerDto.Username);
        result.Token.Should().NotBeNullOrEmpty();

        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be(registerDto.Email);
        savedUser.Username.Should().Be(registerDto.Username);
        savedUser.PasswordHash.Should().NotBeNull();
        savedUser.PasswordSalt.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ThrowsBusinessRuleException()
    {
        // Arrange
        var loginDto = new LoginDto("test@test.com", "password123");
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _sut.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsBusinessRuleException()
    {
        // Arrange
        var loginDto = new LoginDto("test@test.com", "wrongpassword");
        var user = CreateTestUser("testuser", "test@test.com", "correctpassword");
        
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act
        var act = () => _sut.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var password = "password123";
        var user = CreateTestUser("testuser", "test@test.com", password);
        var loginDto = new LoginDto("test@test.com", password);
        
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        result.Username.Should().Be(user.Username);
        result.Token.Should().NotBeNullOrEmpty();
    }

    private User CreateTestUser(string username, string email, string password)
    {
        var user = new User
        {
            Username = username,
            Email = email
        };

        using var hmac = new HMACSHA512();
        user.PasswordSalt = hmac.Key;
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return user;
    }
}
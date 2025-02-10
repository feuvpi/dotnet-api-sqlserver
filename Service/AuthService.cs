using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.DTOS.Auth;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Service;

/// <summary>
/// Handles authentication-related operations, including user registration, login, and JWT token generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="configuration">Configuration for accessing application settings (e.g., JWT secret key).</param>
    /// <param name="userRepository">Repository for accessing user-related data.</param>
    public AuthService(
        IConfiguration configuration,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Registers a new user with the provided details.
    /// </summary>
    /// <param name="registerDto">The user's registration details.</param>
    /// <returns>
    /// An <see cref="AuthResponseDto"/> containing the JWT token, email, and username of the newly registered user.
    /// </returns>
    /// <exception cref="BusinessRuleException">Thrown if the email is already registered.</exception>
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await _userRepository.EmailExistsAsync(registerDto.Email))
            throw new BusinessRuleException("Email already exists");

        var user = new User
        {
            Email = registerDto.Email,
            Username = registerDto.Username
        };

        using var hmac = new HMACSHA512();
        user.PasswordSalt = hmac.Key;
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));

        await _userRepository.AddAsync(user);

        return new AuthResponseDto(
            GenerateJwtToken(user),
            user.Email,
            user.Username);
    }

    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="loginDto">The user's login credentials.</param>
    /// <returns>
    /// An <see cref="AuthResponseDto"/> containing the JWT token, email, and username of the authenticated user.
    /// </returns>
    /// <exception cref="BusinessRuleException">Thrown if the email or password is invalid.</exception>
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
            throw new BusinessRuleException("Invalid email or password");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        if (!computedHash.SequenceEqual(user.PasswordHash))
            throw new BusinessRuleException("Invalid email or password");

        return new AuthResponseDto(
            GenerateJwtToken(user),
            user.Email,
            user.Username);
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <returns>A JWT token as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the JWT secret key is not configured.</exception>
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("JWT:SecretKey not configured")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
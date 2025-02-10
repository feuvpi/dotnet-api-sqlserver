using Core.DTOS.Auth;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api.Controllers;

/// <summary>
/// Handles user authentication, including registration, login, and logout.
/// </summary>
[ApiController]
[Route("api/[controller]")]


public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">Service for handling authentication logic.</param>
    /// <param name="logger">Logger for capturing runtime events and errors.</param>
    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="registerDto">The user's registration details.</param>
    /// <returns>
    /// - 201 Created: If the user is successfully registered. Returns the authentication token and user details.
    /// - 400 Bad Request: If the registration fails due to validation or business rule violations.
    /// - 500 Internal Server Error: If an unexpected error occurs.
    /// </returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(registerDto);
            
            // Return 201 Created instead of 200 OK for resource creation
            return CreatedAtAction(
                nameof(Register), 
                new AuthResponseDto(result.Token, result.Email, result.Username));
        }
        catch (BusinessRuleException ex)
        {
            _logger.LogWarning(ex, "Registration failed for email: {Email}", registerDto.Email);
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for email: {Email}", registerDto.Email);
            return StatusCode(500, new ErrorResponse("An unexpected error occurred during registration."));
        }
    }
    
    /// <summary>
    /// Authenticates a user and generates an authentication token.
    /// </summary>
    /// <param name="loginDto">The user's login credentials.</param>
    /// <returns>
    /// - 200 OK: If the login is successful. Returns the authentication token and user details.
    /// - 401 Unauthorized: If the login credentials are invalid.
    /// - 500 Internal Server Error: If an unexpected error occurs.
    /// </returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            
            // Set auth cookie if you want to support cookie authentication as well
            // You might want to make this configurable
            SetAuthCookie(result.Token);
            
            return Ok(result);
        }
        catch (BusinessRuleException ex)
        {
            _logger.LogWarning(ex, "Login failed for email: {Email}", loginDto.Email);
            return Unauthorized(new ErrorResponse("Invalid credentials"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for email: {Email}", loginDto.Email);
            return StatusCode(500, new ErrorResponse("An unexpected error occurred during login."));
        }
    }

    /// <summary>
    /// Logs out the currently authenticated user by clearing the authentication cookie.
    /// </summary>
    /// <returns>
    /// - 204 No Content: If the logout is successful.
    /// </returns>
    [Authorize]
    [HttpPost("logout")]
    public ActionResult Logout()
    {
        // Clear auth cookie if you're using cookie authentication
        Response.Cookies.Delete("AuthToken");
        return NoContent();
    }

    /// <summary>
    /// Sets an authentication cookie with the provided token.
    /// </summary>
    /// <param name="token">The authentication token to store in the cookie.</param>
    private void SetAuthCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("AuthToken", token, cookieOptions);
    }
}

/// <summary>
/// Represents an error response returned by the API.
/// </summary>
/// <param name="Message">A human-readable error message.</param>
/// <param name="Errors">Optional list of detailed error messages.</param>
public record ErrorResponse(string Message, string[] Errors = null)
{
    public ErrorResponse(string message) : this(message, Array.Empty<string>()) { }
}
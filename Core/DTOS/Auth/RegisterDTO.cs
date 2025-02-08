namespace Core.DTOS.Auth;

public record RegisterDto(
    string Username,
    string Email,
    string Password);
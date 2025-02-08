namespace Core.DTOS.Auth;

public record LoginDto(
    string Email,
    string Password);
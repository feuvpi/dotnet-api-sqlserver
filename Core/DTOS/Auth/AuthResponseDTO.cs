namespace Core.DTOS.Auth;


public record AuthResponseDto(
    string Token,
    string Email,
    string Username);
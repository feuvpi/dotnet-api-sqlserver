namespace Core.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public byte[] PasswordHash { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;
}   
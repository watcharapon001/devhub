namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? Roles { get; set; } // เก็บเป็น "Admin,User" แบบง่ายๆ
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

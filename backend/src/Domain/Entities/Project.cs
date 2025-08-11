namespace Domain.Entities;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? RepoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

namespace Application.Common.Interfaces;

public interface IUserAuth
{
    Task<(Guid id, string username, string? roles)?> FindByUsernameAsync(string username, CancellationToken ct);
    Task<bool> VerifyPasswordAsync(Guid userId, string password, CancellationToken ct);
}

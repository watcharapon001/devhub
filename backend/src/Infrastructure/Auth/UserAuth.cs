using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Infrastructure.Auth;

public class UserAuth : IUserAuth
{
    private readonly AppDbContext _db;
    public UserAuth(AppDbContext db) => _db = db;

    public async Task<(Guid id, string username, string? roles)?> FindByUsernameAsync(string username, CancellationToken ct)
    {
        var u = await _db.Users.AsNoTracking()
                 .Where(x => x.Username == username)
                 .Select(x => new { x.Id, x.Username, x.Roles, x.PasswordHash })
                 .FirstOrDefaultAsync(ct);
        return u is null ? null : (u.Id, u.Username, u.Roles);
    }

    public async Task<bool> VerifyPasswordAsync(Guid userId, string password, CancellationToken ct)
    {
        var hash = await _db.Users.Where(x => x.Id == userId)
                    .Select(x => x.PasswordHash).FirstOrDefaultAsync(ct);
        return hash != null && BCrypt.Net.BCrypt.Verify(password, hash);
    }

    // helper สำหรับ seed
    public static string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
}

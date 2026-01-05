using System.Security.Cryptography;
using System.Text;
using LibrarySystemServer.Data;
using LibrarySystemServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemServer.Services.Auth;

public class RefreshTokenService
{
    private readonly LibrarySystemContext _context;

    public RefreshTokenService(LibrarySystemContext context)
    {
        _context = context;
    }

    private static string GenerateTokenString()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash); // .NET 5+; returns uppercase hex
    }

    public async Task<(string plainToken, RefreshToken entity)> CreateRefreshTokenForUserAsync(string userId, string createdByIp, int daysValid = 30, CancellationToken cancellationToken = default)
    {
        var plain = GenerateTokenString();
        var hash = HashToken(plain);

        var entity = new RefreshToken
        {
            TokenHash = hash,
            UserId = userId,
            Expires = DateTime.UtcNow.AddDays(daysValid),
            CreatedByIp = createdByIp
        };

        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return (plain, entity);
    }

    public async Task<RefreshToken?> GetByHashedTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var hash = HashToken(token);
        return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);
    }

    public async Task RevokeAsync(RefreshToken token, string revokedByIp, string? replacedByHash = null, CancellationToken cancellationToken = default)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = revokedByIp;
        token.ReplacedByTokenHash = replacedByHash;
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllForUserAsync(string userId, string revokedByIp, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens.Where(t => t.UserId == userId && t.IsActive).ToListAsync(cancellationToken);
        foreach (var t in tokens)
        {
            t.Revoked = DateTime.UtcNow;
            t.RevokedByIp = revokedByIp;
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}


using System;

namespace LibrarySystemServer.Model
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByTokenHash { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsExpired && !IsRevoked;
    }
}


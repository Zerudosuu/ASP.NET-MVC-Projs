using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibrarySystemServer.Model;
using Microsoft.IdentityModel.Tokens;

namespace LibrarySystemServer.Services.Auth;

public class JwtTokenService (IConfiguration config)
{
        private readonly IConfiguration _configuration = config;

        public (string token, DateTime expiresAt, string jti) GenerateToken(Member user, IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var jti = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim("role", user.Role.ToString())
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpireMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expiresAt, jti);
        }

} 

public class RevokedToken
{
    public int Id { get; set; }
    public string Jti { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
}
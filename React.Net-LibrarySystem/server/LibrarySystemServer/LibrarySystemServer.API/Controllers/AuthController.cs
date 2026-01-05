using LibrarySystemServer.Model;
using LibrarySystemServer.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace LibrarySystemServer.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;
        private readonly JwtTokenService _jwtService;
        private readonly RefreshTokenService _refreshTokenService;

        public AuthController(UserManager<Member> userManager, JwtTokenService jwtService, RefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null) return Unauthorized("Invalid credentials");

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid) return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAt, jti) = _jwtService.GenerateToken(user, roles);

            // create refresh token and return both
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var (refreshPlain, refreshEntity) = await _refreshTokenService.CreateRefreshTokenForUserAsync(user.Id, ip);

            // Optionally, set refresh token as HttpOnly cookie. Here we return in body for simplicity
            return Ok(new { accessToken = token, expiresAt, jti, refreshToken = refreshPlain });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.RefreshToken)) return BadRequest("Missing refresh token.");

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var existing = await _refreshTokenService.GetByHashedTokenAsync(dto.RefreshToken);
            if (existing == null || !existing.IsActive)
            {
                // optional: if token existed but revoked, treat as possible reuse and revoke all tokens for user
                return Unauthorized("Invalid refresh token.");
            }

            // rotate refresh token: revoke old and create new
            var newPlain = (await _refreshTokenService.CreateRefreshTokenForUserAsync(existing.UserId, ip)).plainToken;
            await _refreshTokenService.RevokeAsync(existing, ip, /* replacedByHash */ null);

            var user = await _userManager.FindByIdAsync(existing.UserId);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAt, jti) = _jwtService.GenerateToken(user, roles);

            return Ok(new { accessToken = token, expiresAt, jti, refreshToken = newPlain });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto)
        {
            // Revoke provided refresh token
            if (!string.IsNullOrEmpty(dto.RefreshToken))
            {
                var existing = await _refreshTokenService.GetByHashedTokenAsync(dto.RefreshToken);
                if (existing != null && existing.IsActive)
                {
                    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                    await _refreshTokenService.RevokeAsync(existing, ip);
                }
            }

            // Optionally revoke current access token by adding its jti to RevokedTokens table
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var tokenString = authHeader[7..].Trim();
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(tokenString))
                {
                    var jwt = handler.ReadJwtToken(tokenString);
                    var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                    var exp = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
                    DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(15);
                    if (long.TryParse(exp, out var expSeconds)) expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;

                    if (!string.IsNullOrEmpty(jti))
                    {
                        // Add to DB via context
                        var ctx = HttpContext.RequestServices.GetRequiredService<LibrarySystemServer.Data.LibrarySystemContext>();
                        ctx.RevokedTokens.Add(new LibrarySystemServer.Services.Auth.RevokedToken { Jti = jti, ExpiresAt = expiresAtUtc });
                        await ctx.SaveChangesAsync();
                    }
                }
            }

            return Ok(new { message = "Logged out" });
        }
    }

    public class LoginDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class LogoutRequestDto
    {
        public string? RefreshToken { get; set; }
    }

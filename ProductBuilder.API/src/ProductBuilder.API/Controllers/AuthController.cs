using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Auth;
using ProductBuilder.Application.Interfaces;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, ITokenService tokenService, IPasswordService passwordService, IConfiguration config)
    {
        _db = db;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var email = request.Email.Trim().ToLower();
        var user = await _db.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.IsActive);

        if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password" });

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var expiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "7");

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpiryMinutes"] ?? "60")),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.Name
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var token = await _db.RefreshTokens.Include(r => r.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken);

        if (token == null || !token.IsActive)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        token.RevokedAt = DateTime.UtcNow;

        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();
        var expiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "7");
        var newRefreshToken = new RefreshToken
        {
            UserId = token.UserId,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };
        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            AccessToken = _tokenService.GenerateAccessToken(token.User),
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpiryMinutes"] ?? "60")),
            User = new UserDto
            {
                Id = token.User.Id,
                Email = token.User.Email,
                FirstName = token.User.FirstName,
                LastName = token.User.LastName,
                Role = token.User.Role.Name
            }
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var forgotEmail = request.Email.Trim().ToLower();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == forgotEmail && u.IsActive);
        if (user == null)
            return Ok(new { message = "If that email is registered, a reset token has been generated." });

        // Invalidate any existing unused tokens for this user
        var existing = await _db.PasswordResetTokens
            .Where(t => t.UserId == user.Id && t.UsedAt == null)
            .ToListAsync();
        foreach (var t in existing)
            t.UsedAt = DateTime.UtcNow;

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();
        _db.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            Token = rawToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        });
        await _db.SaveChangesAsync();

        // No email service in this environment — token is returned directly
        return Ok(new { message = "Reset token generated.", resetToken = rawToken });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var prt = await _db.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.Token);

        if (prt == null || !prt.IsValid)
            return BadRequest(new { message = "Invalid or expired reset token." });

        if (request.NewPassword.Length < 6)
            return BadRequest(new { message = "Password must be at least 6 characters." });

        prt.User.PasswordHash = _passwordService.HashPassword(request.NewPassword);
        prt.User.UpdatedAt = DateTime.UtcNow;
        prt.UsedAt = DateTime.UtcNow;

        // Revoke all active refresh tokens so existing sessions are invalidated
        var refreshTokens = await _db.RefreshTokens
            .Where(r => r.UserId == prt.UserId && r.RevokedAt == null)
            .ToListAsync();
        foreach (var rt in refreshTokens)
            rt.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Password reset successfully. Please sign in with your new password." });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == request.RefreshToken);
        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return NoContent();
    }
}

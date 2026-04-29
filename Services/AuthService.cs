using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TurfArena.Data;
using TurfArena.DTOs.Auth;
using TurfArena.Models;

namespace TurfArena.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <summary>Register a new user</summary>
    public async Task<(bool Success, string Message, AppUser? User)> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email.ToLower()))
            return (false, "Email already registered.", null);

        var user = new AppUser
        {
            Name = dto.Name,
            Email = dto.Email.ToLower(),
            Phone = dto.Phone,
            Role = dto.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return (true, "Registration successful.", user);
    }

    /// <summary>Validate credentials and return the user</summary>
    public async Task<AppUser?> ValidateAsync(LoginDto dto)
    {
        var user = await _db.Users
            .Include(u => u.FavoriteTurfs)
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;
        return user;
    }

    /// <summary>Generate a JWT token for the given user</summary>
    public string GenerateToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(
            double.Parse(_config["Jwt:ExpiryInMinutes"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name),
            new Claim("role", user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>Map AppUser → UserProfileDto</summary>
    public UserProfileDto ToProfileDto(AppUser user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            ProfilePicture = user.ProfilePicture,
            PushToken = user.PushToken,
            FavoriteTurfs = user.FavoriteTurfs.Select(f => f.TurfId).ToList(),
            CreatedAt = user.CreatedAt
        };
    }
}

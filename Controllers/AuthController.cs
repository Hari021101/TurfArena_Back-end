using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurfArena.Data;
using TurfArena.DTOs.Auth;
using TurfArena.Services;

namespace TurfArena.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly AppDbContext _db;

    public AuthController(AuthService authService, AppDbContext db)
    {
        _authService = authService;
        _db = db;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });

        var (success, message, user) = await _authService.RegisterAsync(dto);
        if (!success) return Conflict(new { message });

        var token = _authService.GenerateToken(user!);
        return Ok(new LoginResponseDto
        {
            Token = token,
            User = _authService.ToProfileDto(user!)
        });
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _authService.ValidateAsync(dto);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password." });

        var token = _authService.GenerateToken(user);
        return Ok(new LoginResponseDto
        {
            Token = token,
            User = _authService.ToProfileDto(user)
        });
    }

    // GET /api/auth/me
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");
        if (userId == null) return Unauthorized();

        var user = await _db.Users
            .Include(u => u.FavoriteTurfs)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound();
        return Ok(_authService.ToProfileDto(user));
    }

    // PUT /api/auth/profile
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (dto.Name != null) user.Name = dto.Name;
        if (dto.Phone != null) user.Phone = dto.Phone;
        if (dto.ProfilePicture != null) user.ProfilePicture = dto.ProfilePicture;
        if (dto.PushToken != null) user.PushToken = dto.PushToken;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Profile updated successfully." });
    }
}

using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Services;
using ApiCatalog.Application.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService auth) : ControllerBase
{
    // ---------------- Register ----------------
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var created = await auth.RegisterAsync(dto);
        if (!created)
            return BadRequest(new { message = Messages.Auth.UserAlreadyExists });

        return Ok(new { message = Messages.Auth.UserRegistered });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var tokenOrNull = await auth.LoginAsync(dto);
        var user = await auth.GetUserByEmailOrUsernameAsync(dto.Email); 

        if (user is null || user.Role is null)
            return Unauthorized(new { message = Messages.Auth.InvalidCredentials });

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email ?? user.Username ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("role_level", user.Role.Level.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                AllowRefresh = true
            });

        return Ok(new
        {
            message = Messages.Auth.UserRegistered,
            jwt = tokenOrNull
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = Messages.Logout.LogoutSuccessful });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            emailOrName = User.Identity?.Name,
            role = User.FindFirstValue(ClaimTypes.Role),
            roleLevel = User.FindFirstValue("role_level")
        });
    }
}

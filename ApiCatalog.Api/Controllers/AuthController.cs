using System.Security.Claims;
using ApiCatalog.Api.Extensions;
using ApiCatalog.Application.Common;
using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (success, message) = await auth.RegisterAsync(dto);
        
        return !success ? this.BadRequestWithMessage(message ?? Messages.Auth.UserAlreadyExists) : this.CreatedWithMessage<object>(nameof(Me), null, null, message ?? Messages.Auth.UserRegistered);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (token, message) = await auth.LoginAsync(dto);
        if (token == null)
            return this.BadRequestWithMessage(message ?? Messages.Auth.InvalidCredentials);

        var user = await auth.GetUserByEmailOrUsernameAsync(dto.Email);

        if (user?.Role is null)
            return this.BadRequestWithMessage(Messages.Auth.InvalidCredentials);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email ?? user.Username ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("role_level", user.Role.Level.ToString())
        };

        const string cookieScheme = "SmartCookie";
        var identity = new ClaimsIdentity(claims, cookieScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            cookieScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                AllowRefresh = true
            });

        return this.OkWithMessage(new { jwt = token }, message ?? Messages.Auth.LoginSuccessful);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        const string cookieScheme = "SmartCookie";
        await HttpContext.SignOutAsync(cookieScheme);
        return this.OkWithMessage<object?>(null, Messages.Logout.LogoutSuccessful);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
            return this.BadRequestWithMessage(Messages.Auth.Unauthenticated);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var emailOrName = User.Identity?.Name;
        var role = User.FindFirstValue(ClaimTypes.Role);
        var roleLevel = User.FindFirstValue("role_level");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(roleLevel))
            return this.BadRequestWithMessage(Messages.Auth.MissingClaims);

        var userData = new
        {
            userId,
            emailOrName,
            role,
            roleLevel
        };

        return this.OkWithMessage(userData, Messages.JsonResponsesApi.Success);
    }
}

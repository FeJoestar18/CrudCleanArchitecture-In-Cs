using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ApiCatalog.Application.Common;

namespace ApiCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService auth) : ControllerBase
{
    private readonly AuthService _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var created = await _auth.RegisterAsync(dto);
        if (!created) return BadRequest(new { message = Messages.Auth.UserAlreadyExists });
        
        return Ok(new { message = Messages.Auth.UserRegistered });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _auth.LoginAsync(dto);
        if (token == null) return Unauthorized(new { message = Messages.Auth.InvalidCredentials });
        
        return Ok(new { token });
    }
}
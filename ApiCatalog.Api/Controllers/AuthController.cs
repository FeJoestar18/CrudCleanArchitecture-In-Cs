using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
        if (!created) return BadRequest(new { message = $"Usuário já existe" });
        return Ok(new { message = "Usuário registrado" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _auth.LoginAsync(dto);
        if (token == null) return Unauthorized(new { message = "Usuário ou senha inválidos" });
        return Ok(new { token });
    }
}
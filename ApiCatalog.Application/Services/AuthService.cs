using ApiCatalog.Application.DTOs;
using ApiCatalog.Domain.Interfaces;
using ApiCatalog.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ApiCatalog.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _configuration = configuration;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var existing = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existing != null) return false;

        // Buscar role por nome ou usar "Usuario" como padrão
        Role? role = null;
        if (!string.IsNullOrEmpty(dto.Role))
        {
            role = await _roleRepository.GetByNameAsync(dto.Role);
        }
        
        // Se não encontrou ou não foi especificado, usar role padrão "Usuario"
        role ??= await _roleRepository.GetByNameAsync("Usuario");
        
        if (role == null) return false; // Role padrão deve existir

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Cpf = dto.Cpf,
            Email = dto.Email,
            RoleId = role.Id
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Email);
        if (user == null || user.Role == null) return null;

        var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!valid) return null;
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("role_level", user.Role.Level.ToString())
        };

        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

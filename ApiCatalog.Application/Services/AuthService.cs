using ApiCatalog.Application.DTOs;
using ApiCatalog.Domain.Interfaces;
using ApiCatalog.Domain.Entities;
using ApiCatalog.Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ApiCatalog.Application.Services;

public class AuthService(IUserRepository userRepository, IConfiguration configuration)
{
    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var existing = await userRepository.GetByUsernameAsync(dto.Username);
        if (existing != null) return false; // já existe

        var rolePermissoes = RolePermissoes.Usuario; // padrão
        if (!string.IsNullOrEmpty(dto.Role) && Enum.TryParse<RolePermissoes>(dto.Role, true, out var parsedRole))
        {
            rolePermissoes = parsedRole;
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Cpf = dto.Cpf,
            Email = dto.Email,
            RolePermissoes = rolePermissoes
        };

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        var user = await userRepository.GetByUsernameAsync(dto.Username);
        if (user == null) return null;

        var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!valid) return null;

        // Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
            new Claim(ClaimTypes.Role, user.RolePermissoes.ToString())
        };

        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

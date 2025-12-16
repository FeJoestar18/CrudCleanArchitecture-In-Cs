using ApiCatalog.Application.DTOs;
using ApiCatalog.Domain.Interfaces;
using ApiCatalog.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ApiCatalog.Application.Services;

public class AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration)
{
    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var existing = await userRepository.GetByUsernameAsync(dto.Username);
        if (existing != null) return false;

        Role? role = null;
        if (!string.IsNullOrEmpty(dto.Role))
        {
            role = await roleRepository.GetByNameAsync(dto.Role);
        }
        
        role ??= await roleRepository.GetByNameAsync("Usuario");
        
        if (role == null) return false; 

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Cpf = dto.Cpf,
            Email = dto.Email,
            RoleId = role.Id
        };

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        var user = await userRepository.GetByUsernameAsync(dto.Email);
        if (user?.Role == null) return null;

        var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!valid) return null;
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("role_level", user.Role.Level.ToString())
        };

        var keyString = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(keyString) || Encoding.UTF8.GetByteCount(keyString) < 16)
        {
            throw new InvalidOperationException("JWT key is missing or too short. Provide a symmetric key with at least 128 bits (16 bytes).");
        }
        var key = Encoding.UTF8.GetBytes(keyString);
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
    
    public async Task<User?> GetUserByEmailOrUsernameAsync(string emailOrUsername)
    {
        var user = await userRepository.GetByEmailAsync(emailOrUsername);
        if (user != null) return user;
        return await userRepository.GetByUsernameAsync(emailOrUsername);
    }
}

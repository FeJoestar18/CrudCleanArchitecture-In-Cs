namespace ApiCatalog.Application.DTOs;

public record RegisterDto(string Username, string Password, string Cpf, string? Email, string? Role = null);

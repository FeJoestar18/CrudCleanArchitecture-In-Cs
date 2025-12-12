namespace ApiCatalog.Application.DTOs;

public record RegisterDto(string Username, string Password, int Cpf, string? Email, string? Role = null);

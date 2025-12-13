namespace ApiCatalog.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public int Cpf { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public int RoleId { get; set; }
    public Role? Role { get; set; }
}
using System.Threading.Tasks;
using ApiCatalog.Domain.Entities;

namespace ApiCatalog.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
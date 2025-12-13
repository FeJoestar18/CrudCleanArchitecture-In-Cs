using System.Threading.Tasks;
using ApiCatalog.Domain.Entities;

namespace ApiCatalog.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<User?> GetByEmailAsync(string emailOrUsername);
}
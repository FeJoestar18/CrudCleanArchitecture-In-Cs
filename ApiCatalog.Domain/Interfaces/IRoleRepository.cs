using ApiCatalog.Domain.Entities;

namespace ApiCatalog.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<List<Role>> GetAllAsync();
    Task AddAsync(Role role);
    Task SaveChangesAsync();
}


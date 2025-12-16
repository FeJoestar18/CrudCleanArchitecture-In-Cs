using ApiCatalog.Domain.Entities;
using ApiCatalog.Domain.Interfaces;
using ApiCatalog.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalog.Infra.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<User?> GetByCpfAsync(string cpf)
    {
        return await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Cpf == cpf);
    }
    
    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
    }
    
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
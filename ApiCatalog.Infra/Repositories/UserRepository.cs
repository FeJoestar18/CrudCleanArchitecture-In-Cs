using ApiCatalog.Domain.Entities;
using ApiCatalog.Domain.Interfaces;
using ApiCatalog.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalog.Infra.Repositories;

public class UserRepository(AppDbContext context, IUserRepository? userRepositoryImplementation) : IUserRepository
{
    private readonly IUserRepository? _userRepositoryImplementation = userRepositoryImplementation;

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);
    }
    
    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
    }
    
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public Task<User?> GetByEmailAsync(string emailOrUsername)
    {
        return _userRepositoryImplementation.GetByEmailAsync(emailOrUsername);
    }
}
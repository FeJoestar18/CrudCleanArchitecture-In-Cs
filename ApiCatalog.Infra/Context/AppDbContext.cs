using Microsoft.EntityFrameworkCore;
using ApiCatalog.Domain.Entities;

namespace ApiCatalog.Infra.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ApiCatalog.Domain.Entities.User> Users => Set<ApiCatalog.Domain.Entities.User>();
}
using Microsoft.EntityFrameworkCore;
using ApiCatalog.Domain.Entities;

namespace ApiCatalog.Infra.Context;
public class AppDbContext : DbContext
{ 
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}
    public DbSet<Product> Products => Set<Product>();
    
}
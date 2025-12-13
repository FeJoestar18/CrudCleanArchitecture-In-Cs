using Microsoft.EntityFrameworkCore;
using ApiCatalog.Domain.Entities;

namespace ApiCatalog.Infra.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<UserProduct> UserProducts => Set<UserProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar relacionamento User -> Role
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar auto-referência Role -> ParentRole
        modelBuilder.Entity<Role>()
            .HasOne(r => r.ParentRole)
            .WithMany()
            .HasForeignKey(r => r.ParentRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar relacionamento Product -> CreatedBy
        modelBuilder.Entity<Product>()
            .HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar relacionamento Product -> RequestedDeletionBy
        modelBuilder.Entity<Product>()
            .HasOne(p => p.RequestedDeletionBy)
            .WithMany()
            .HasForeignKey(p => p.RequestedDeletionByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar relacionamento UserProduct
        modelBuilder.Entity<UserProduct>()
            .HasOne(up => up.User)
            .WithMany()
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserProduct>()
            .HasOne(up => up.Product)
            .WithMany()
            .HasForeignKey(up => up.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed inicial de Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Usuario", Level = 1, ParentRoleId = null },
            new Role { Id = 2, Name = "Funcionario", Level = 2, ParentRoleId = 1 },
            new Role { Id = 3, Name = "Admin", Level = 3, ParentRoleId = 2 }
        );
    }
}
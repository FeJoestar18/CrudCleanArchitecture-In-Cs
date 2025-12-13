using ApiCatalog.Domain.Entities;
using ApiCatalog.Domain.Interfaces;
using ApiCatalog.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalog.Infra.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.CreatedBy)
            .Include(p => p.RequestedDeletionBy)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Products
            .Include(p => p.CreatedBy)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive && !p.PendingDeletion);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<List<Product>> GetPendingDeletionAsync()
    {
        return await _context.Products
            .Include(p => p.RequestedDeletionBy)
            .Where(p => p.PendingDeletion)
            .OrderBy(p => p.DeletionRequestedAt)
            .ToListAsync();
    }

    public async Task<List<UserProduct>> GetUserProductsAsync(int userId)
    {
        return await _context.UserProducts
            .Include(up => up.Product)
            .Where(up => up.UserId == userId)
            .OrderByDescending(up => up.PurchasedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await Task.CompletedTask;
    }

    public async Task<UserProduct?> AddUserProductAsync(UserProduct userProduct)
    {
        await _context.UserProducts.AddAsync(userProduct);
        return userProduct;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}


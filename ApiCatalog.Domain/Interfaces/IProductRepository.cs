using ApiCatalog.Domain.Entities;

namespace ApiCatalog.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync(bool includeInactive = false);
    Task<List<Product>> GetPendingDeletionAsync();
    Task<List<UserProduct>> GetUserProductsAsync(int userId);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<UserProduct?> AddUserProductAsync(UserProduct userProduct);
    Task SaveChangesAsync();
}


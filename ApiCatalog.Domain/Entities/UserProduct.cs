namespace ApiCatalog.Domain.Entities;

/// <summary>
/// Representa produtos adquiridos por usuários
/// </summary>
public class UserProduct
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
}


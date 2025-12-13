namespace ApiCatalog.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public User? CreatedBy { get; set; }
    
    // Para controle de deleção por funcionários
    public bool PendingDeletion { get; set; } = false;
    public int? RequestedDeletionByUserId { get; set; }
    public User? RequestedDeletionBy { get; set; }
    public DateTime? DeletionRequestedAt { get; set; }
}


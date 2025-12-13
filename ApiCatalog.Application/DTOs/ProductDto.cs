namespace ApiCatalog.Application.DTOs;

public record CreateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int Stock
);

public record UpdateProductDto(
    string? Name,
    string? Description,
    decimal? Price,
    int? Stock,
    bool? IsActive
);

public record PurchaseProductDto(
    int ProductId,
    int Quantity
);

public record ProductResponseDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? CreatedByUsername,
    bool PendingDeletion,
    string? RequestedDeletionByUsername,
    DateTime? DeletionRequestedAt
);

public record UserProductResponseDto(
    int Id,
    string ProductName,
    int Quantity,
    decimal PurchasePrice,
    DateTime PurchasedAt
);


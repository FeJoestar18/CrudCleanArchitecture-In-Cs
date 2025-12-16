using ApiCatalog.Application.Common;
using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Policies;
using ApiCatalog.Domain.Entities;
using ApiCatalog.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ApiCatalog.Application.Services;

public class ProductService(
    IProductRepository productRepository,
    IUserRepository userRepository,
    IAuthorizationService authorizationService)
{
    private static decimal RoundPrice(decimal price) => Math.Round(price, 2);
    public async Task<List<ProductResponseDto>> GetAllProductsAsync(ClaimsPrincipal user)
    {
        var isAdmin = await IsUserLevelAsync(user, 3);
        var products = await productRepository.GetAllAsync(includeInactive: isAdmin);

        return products.Select(p => new ProductResponseDto(
            p.Id,
            p.Name,
            p.Description,
            RoundPrice(p.Price),
            p.Stock,
            p.IsActive,
            p.CreatedAt,
            p.UpdatedAt,
            p.CreatedBy?.Username,
            p.PendingDeletion,
            p.RequestedDeletionBy?.Username,
            p.DeletionRequestedAt
        )).ToList();
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null) return null;

        return new ProductResponseDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt,
            product.CreatedBy?.Username,
            product.PendingDeletion,
            product.RequestedDeletionBy?.Username,
            product.DeletionRequestedAt
        );
    }

    public async Task<(bool Success, string Message, ProductResponseDto? Product)> CreateProductAsync(
        ClaimsPrincipal user, 
        CreateProductDto dto)
    {
        if (!await IsUserLevelAsync(user, 2))
        {
            return (false, Messages.Products.OnlyEmployeesCanCreate, null);
        }

        var currentUser = await GetCurrentUserAsync(user);
        if (currentUser == null)
        {
            return (false, Messages.Auth.UserNotFound, null);
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = RoundPrice(dto.Price),
            Stock = dto.Stock,
            CreatedByUserId = currentUser.Id,
            IsActive = true
        };

        await productRepository.AddAsync(product);
        await productRepository.SaveChangesAsync();

        var response = new ProductResponseDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt,
            currentUser.Username,
            false,
            null,
            null
        );

        return (true, Messages.Products.ProductCreated, response);
    }

    public async Task<(bool Success, string Message)> UpdateProductAsync(
        ClaimsPrincipal user,
        int id,
        UpdateProductDto dto)
    {
        // Validar permissão mínima (Funcionario - Level 2)
        if (!await IsUserLevelAsync(user, 2))
        {
            return (false, Messages.Products.OnlyEmployeesCanEdit);
        }

        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return (false, Messages.Products.ProductNotFound);
        }

        if (product.PendingDeletion)
        {
            return (false, Messages.Products.CannotEditPendingDeletion);
        }

        if (!string.IsNullOrEmpty(dto.Name))
            product.Name = dto.Name;

        if (dto.Description != null)
            product.Description = dto.Description;

        if (dto.Price.HasValue)
            product.Price = RoundPrice(dto.Price.Value);

        if (dto.Stock.HasValue)
            product.Stock = dto.Stock.Value;

        if (dto.IsActive.HasValue)
            product.IsActive = dto.IsActive.Value;

        await productRepository.UpdateAsync(product);
        await productRepository.SaveChangesAsync();

        return (true, Messages.Products.ProductUpdated);
    }

    public async Task<(bool Success, string Message)> DeleteProductAsync(
        ClaimsPrincipal user,
        int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return (false, Messages.Products.ProductNotFound);
        }

        var isAdmin = await IsUserLevelAsync(user, 3);
        var isFuncionario = await IsUserLevelAsync(user, 2);

        if (!isFuncionario && !isAdmin)
        {
            return (false, Messages.Products.OnlyEmployeesCanDelete);
        }

        if (isAdmin)
        {
            await productRepository.DeleteAsync(product);
            await productRepository.SaveChangesAsync();
            return (true, Messages.Products.ProductDeleted);
        }

        var currentUser = await GetCurrentUserAsync(user);
        if (currentUser == null)
        {
            return (false, Messages.Auth.UserNotFound);
        }

        product.PendingDeletion = true;
        product.RequestedDeletionByUserId = currentUser.Id;
        product.DeletionRequestedAt = DateTime.UtcNow;

        await productRepository.UpdateAsync(product);
        await productRepository.SaveChangesAsync();

        return (true, Messages.Products.DeletionRequested);
    }

    public async Task<(bool Success, string Message, List<ProductResponseDto>? Products)> GetPendingDeletionAsync(
        ClaimsPrincipal user)
    {
        if (!await IsUserLevelAsync(user, 3))
        {
            return (false, Messages.Products.OnlyAdminsCanViewPending, null);
        }

        var products = await productRepository.GetPendingDeletionAsync();
        var response = products.Select(p => new ProductResponseDto(
            p.Id,
            p.Name,
            p.Description,
            RoundPrice(p.Price),
            p.Stock,
            p.IsActive,
            p.CreatedAt,
            p.UpdatedAt,
            p.CreatedBy?.Username,
            p.PendingDeletion,
            p.RequestedDeletionBy?.Username,
            p.DeletionRequestedAt
        )).ToList();

        return (true, Messages.Products.PendingDeletionProducts, response);
    }

    public async Task<(bool Success, string Message)> ApproveDeletionAsync(
        ClaimsPrincipal user,
        int id)
    {
        if (!await IsUserLevelAsync(user, 3))
        {
            return (false, Messages.Products.OnlyAdminsCanApprove);
        }

        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return (false, Messages.Products.ProductNotFound);
        }

        if (!product.PendingDeletion)
        {
            return (false, Messages.Products.NotPendingDeletion);
        }

        await productRepository.DeleteAsync(product);
        await productRepository.SaveChangesAsync();

        return (true, Messages.Products.DeletionApproved);
    }

    public async Task<(bool Success, string Message)> RejectDeletionAsync(
        ClaimsPrincipal user,
        int id)
    {
        if (!await IsUserLevelAsync(user, 3))
        {
            return (false, Messages.Products.OnlyAdminsCanReject);
        }

        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return (false, Messages.Products.ProductNotFound);
        }

        if (!product.PendingDeletion)
        {
            return (false, Messages.Products.NotPendingDeletion);
        }

        product.PendingDeletion = false;
        product.RequestedDeletionByUserId = null;
        product.DeletionRequestedAt = null;

        await productRepository.UpdateAsync(product);
        await productRepository.SaveChangesAsync();

        return (true, Messages.Products.DeletionRejected);
    }

    public async Task<(bool Success, string Message, UserProductResponseDto? Purchase)> PurchaseProductAsync(
        ClaimsPrincipal user,
        PurchaseProductDto dto)
    {
        var currentUser = await GetCurrentUserAsync(user);
        if (currentUser == null)
        {
            return (false, Messages.Auth.UserNotFound, null);
        }

        var product = await productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
        {
            return (false, Messages.Products.ProductNotFound, null);
        }

        if (!product.IsActive || product.PendingDeletion)
        {
            return (false, Messages.Products.ProductUnavailable, null);
        }

        if (product.Stock < dto.Quantity)
        {
            return (false, string.Format(Messages.Products.InsufficientStock, product.Stock), null);
        }

        product.Stock -= dto.Quantity;
        await productRepository.UpdateAsync(product);

        var userProduct = new UserProduct
        {
            UserId = currentUser.Id,
            ProductId = product.Id,
            Quantity = dto.Quantity,
            PurchasePrice = RoundPrice(product.Price)
        };

        await productRepository.AddUserProductAsync(userProduct);
        await productRepository.SaveChangesAsync();

        var response = new UserProductResponseDto(
            userProduct.Id,
            product.Name,
            dto.Quantity,
            RoundPrice(product.Price),
            userProduct.PurchasedAt
        );

        return (true, Messages.Products.ProductPurchased, response);
    }

    public async Task<List<UserProductResponseDto>> GetMyProductsAsync(ClaimsPrincipal user)
    {
        var currentUser = await GetCurrentUserAsync(user);
        if (currentUser == null) return [];

        var userProducts = await productRepository.GetUserProductsAsync(currentUser.Id);
        
        return userProducts.Select(up => new UserProductResponseDto(
            up.Id,
            up.Product?.Name ?? Messages.Products.ProductNotFound,
            up.Quantity,
            RoundPrice(up.PurchasePrice),
            up.PurchasedAt
        )).ToList();
    }

    private async Task<bool> IsUserLevelAsync(ClaimsPrincipal user, int requiredLevel)
    {
        var authResult = await authorizationService.AuthorizeAsync(
            user,
            null,
            new MinimumRoleLevelRequirement(requiredLevel)
        );
        return authResult.Succeeded;
    }

    private async Task<User?> GetCurrentUserAsync(ClaimsPrincipal user)
    {
        var email = user.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(email)) return null;

        return await userRepository.GetByUsernameAsync(email);
    }
}


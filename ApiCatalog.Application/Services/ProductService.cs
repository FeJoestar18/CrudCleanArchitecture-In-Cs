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
    public async Task<List<ProductResponseDto>> GetAllProductsAsync(ClaimsPrincipal user)
    {
        var isAdmin = await IsUserLevelAsync(user, 3);
        var products = await productRepository.GetAllAsync(includeInactive: isAdmin);

        return products.Select(p => new ProductResponseDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
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
            return (false, "Apenas funcionários ou admins podem criar produtos", null);
        }

        var currentUser = await GetCurrentUserAsync(user);
        if (currentUser == null)
        {
            return (false, "Usuário não encontrado", null);
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
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

        return (true, "Produto criado com sucesso", response);
    }

    public async Task<(bool Success, string Message)> UpdateProductAsync(
        ClaimsPrincipal user,
        int id,
        UpdateProductDto dto)
    {
        // Validar permissão mínima (Funcionario - Level 2)
        if (!await IsUserLevelAsync(user, 2))
        {
            return (false, "Apenas funcionários ou admins podem editar produtos");
        }

        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return (false, "Produto não encontrado");
        }

        if (product.PendingDeletion)
        {
            return (false, "Produto pendente de deleção não pode ser editado");
        }

        // Atualizar campos
        if (!string.IsNullOrEmpty(dto.Name))
            product.Name = dto.Name;

        if (dto.Description != null)
            product.Description = dto.Description;

        if (dto.Price.HasValue)
            product.Price = dto.Price.Value;

        if (dto.Stock.HasValue)
            product.Stock = dto.Stock.Value;

        if (dto.IsActive.HasValue)
            product.IsActive = dto.IsActive.Value;

        await productRepository.UpdateAsync(product);
        await productRepository.SaveChangesAsync();

        return (true, "Produto atualizado com sucesso");
    }

    public async Task<(bool Success, string Message)> DeleteProductAsync(
        ClaimsPrincipal user,
        int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return (false, "Produto não encontrado");
        }

        var isAdmin = await IsUserLevelAsync(user, 3);
        var isFuncionario = await IsUserLevelAsync(user, 2);

        if (!isFuncionario && !isAdmin)
        {
            return (false, "Você não tem permissão para deletar produtos");
        }

        if (isAdmin)
        {
            await productRepository.DeleteAsync(product);
            await productRepository.SaveChangesAsync();
            return (true, "Produto deletado com sucesso");
        }

        var currentUser = await GetCurrentUserAsync(user);
        if (currentUser == null)
        {
            return (false, "Usuário não encontrado");
        }

        product.PendingDeletion = true;
        product.RequestedDeletionByUserId = currentUser.Id;
        product.DeletionRequestedAt = DateTime.UtcNow;

        await productRepository.UpdateAsync(product);
        await productRepository.SaveChangesAsync();

        return (true, "Solicitação de deleção enviada para aprovação do admin");
    }

    public async Task<(bool Success, string Message, List<ProductResponseDto>? Products)> GetPendingDeletionAsync(
        ClaimsPrincipal user)
    {
        if (!await IsUserLevelAsync(user, 3))
        {
            return (false, "Apenas admins podem visualizar solicitações de deleção", null);
        }

        var products = await productRepository.GetPendingDeletionAsync();
        var response = products.Select(p => new ProductResponseDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Stock,
            p.IsActive,
            p.CreatedAt,
            p.UpdatedAt,
            p.CreatedBy?.Username,
            p.PendingDeletion,
            p.RequestedDeletionBy?.Username,
            p.DeletionRequestedAt
        )).ToList();

        return (true, "Produtos pendentes de deleção", response);
    }

    public async Task<(bool Success, string Message)> ApproveDeletionAsync(
        ClaimsPrincipal user,
        int id)
    {
        if (!await IsUserLevelAsync(user, 3))
        {
            return (false, "Apenas admins podem aprovar deleções");
        }

        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return (false, "Produto não encontrado");
        }

        if (!product.PendingDeletion)
        {
            return (false, "Este produto não está pendente de deleção");
        }

        await productRepository.DeleteAsync(product);
        await productRepository.SaveChangesAsync();

        return (true, "Deleção aprovada e produto removido");
    }

    public async Task<(bool Success, string Message)> RejectDeletionAsync(
        ClaimsPrincipal user,
        int id)
    {
        if (!await IsUserLevelAsync(user, 3))
        {
            return (false, "Apenas admins podem rejeitar deleções");
        }

        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return (false, "Produto não encontrado");
        }

        if (!product.PendingDeletion)
        {
            return (false, "Este produto não está pendente de deleção");
        }

        product.PendingDeletion = false;
        product.RequestedDeletionByUserId = null;
        product.DeletionRequestedAt = null;

        await productRepository.UpdateAsync(product);
        await productRepository.SaveChangesAsync();

        return (true, "Solicitação de deleção rejeitada");
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
            PurchasePrice = product.Price
        };

        await productRepository.AddUserProductAsync(userProduct);
        await productRepository.SaveChangesAsync();

        var response = new UserProductResponseDto(
            userProduct.Id,
            product.Name,
            dto.Quantity,
            product.Price,
            userProduct.PurchasedAt
        );

        return (true, Messages.Products.ProductPurchased, response);
    }

    public async Task<List<UserProductResponseDto>> GetMyProductsAsync(ClaimsPrincipal user)
    {
        var currentUser = await GetCurrentUserAsync(user);
        if (currentUser == null) return new List<UserProductResponseDto>();

        var userProducts = await productRepository.GetUserProductsAsync(currentUser.Id);
        
        return userProducts.Select(up => new UserProductResponseDto(
            up.Id,
            up.Product?.Name ?? "Produto não encontrado",
            up.Quantity,
            up.PurchasePrice,
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


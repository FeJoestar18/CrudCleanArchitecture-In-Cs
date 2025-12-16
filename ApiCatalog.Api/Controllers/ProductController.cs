using ApiCatalog.Api.Extensions;
using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiCatalog.Application.Common;

namespace ApiCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController(ProductService productService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> GetAll()
    {
        var products = await productService.GetAllProductsAsync(User);
        return this.OkWithMessage(products, Messages.JsonResponsesApi.Success);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await productService.GetProductByIdAsync(id);
        return product == null ? this.NotFoundWithMessage(Messages.Products.ProductNotFound) : this.OkWithMessage(product, Messages.JsonResponsesApi.Success);
    }

    [HttpPost]
    [Authorize(Policy = "FuncionarioOrAbove")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var (success, message, product) = await productService.CreateProductAsync(User, dto);
        
        return !success ? this.BadRequestWithMessage(message) : this.CreatedWithMessage(nameof(GetById), new { id = product!.Id }, product, Messages.Products.ProductCreated);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "FuncionarioOrAbove")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto? dto)
    {
        if (dto == null)
            return this.BadRequestWithMessage(Messages.JsonResponsesApi.InvalidJson);

        if (!ModelState.IsValid)
            return this.BadRequestWithMessage(Messages.JsonResponsesApi.InvalidRequest);

        var (success, message) = await productService.UpdateProductAsync(User, id, dto);
    
        return !success ? this.BadRequestWithMessage(message) : this.NoContentWithMessage(Messages.Products.ProductUpdated);
    }
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "FuncionarioOrAbove")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await productService.DeleteProductAsync(User, id);
        
        return !success ? this.BadRequestWithMessage(message) : this.OkWithMessage<object?>(null, message);
    }

    [HttpGet("pending-deletion")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetPendingDeletion()
    {
        var (success, message, products) = await productService.GetPendingDeletionAsync(User);
        
        return !success ? this.ForbidWithMessage(message) : this.OkWithMessage(products, message);
    }

    [HttpPost("{id:int}/approve-deletion")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ApproveDeletion(int id)
    {
        var (success, message) = await productService.ApproveDeletionAsync(User, id);
        
        return !success ? this.BadRequestWithMessage(message) : this.OkWithMessage<object?>(null, message);
    }

    [HttpPost("{id:int}/reject-deletion")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RejectDeletion(int id)
    {
        var (success, message) = await productService.RejectDeletionAsync(User, id);
        
        return !success ? this.BadRequestWithMessage(message) : this.OkWithMessage<object?>(null, message);
    }

    [HttpPost("purchase")]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseProductDto dto)
    {
        var (success, message, purchase) = await productService.PurchaseProductAsync(User, dto);
        
        return !success ? this.BadRequestWithMessage(message) : this.OkWithMessage(purchase, message);
    }

    [HttpGet("my-products")]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> GetMyProducts()
    {
        var products = await productService.GetMyProductsAsync(User);
        return this.OkWithMessage(products, Messages.JsonResponsesApi.Success);
    }
}


using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiCatalog.Application.Common;

namespace ApiCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllProductsAsync(User);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { message = Messages.Products.ProductNotFound });
        }
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Policy = "FuncionarioOrAbove")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var (success, message, product) = await _productService.CreateProductAsync(User, dto);
        
        if (!success)
        {
            return BadRequest(new { message });
        }

        return CreatedAtAction(nameof(GetById), new { id = product!.Id }, product);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "FuncionarioOrAbove")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var (success, message) = await _productService.UpdateProductAsync(User, id, dto);
        
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "FuncionarioOrAbove")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _productService.DeleteProductAsync(User, id);
        
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpGet("pending-deletion")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetPendingDeletion()
    {
        var (success, _, products) = await _productService.GetPendingDeletionAsync(User);
        
        if (!success)
        {
            return Forbid();
        }

        return Ok(products);
    }

    [HttpPost("{id:int}/approve-deletion")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ApproveDeletion(int id)
    {
        var (success, message) = await _productService.ApproveDeletionAsync(User, id);
        
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpPost("{id:int}/reject-deletion")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RejectDeletion(int id)
    {
        var (success, message) = await _productService.RejectDeletionAsync(User, id);
        
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }

    [HttpPost("purchase")]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseProductDto dto)
    {
        var (success, message, purchase) = await _productService.PurchaseProductAsync(User, dto);
        
        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message, purchase });
    }

    [HttpGet("my-products")]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> GetMyProducts()
    {
        var products = await _productService.GetMyProductsAsync(User);
        return Ok(products);
    }
}


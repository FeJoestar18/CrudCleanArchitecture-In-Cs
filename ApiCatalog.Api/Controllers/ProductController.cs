using ApiCatalog.Application.DTOs;
using ApiCatalog.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    /// <summary>
    /// Lista todos os produtos (todos podem visualizar)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllProductsAsync(User);
        return Ok(products);
    }

    /// <summary>
    /// Busca produto por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { message = "Produto não encontrado" });
        }
        return Ok(product);
    }

    /// <summary>
    /// Cria produto (Funcionario e Admin)
    /// </summary>
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

    /// <summary>
    /// Atualiza produto (Funcionario e Admin)
    /// </summary>
    [HttpPut("{id}")]
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

    /// <summary>
    /// Deleta produto
    /// - Admin: deleta imediatamente
    /// - Funcionario: solicita aprovação
    /// </summary>
    [HttpDelete("{id}")]
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

    /// <summary>
    /// Lista produtos pendentes de deleção (apenas Admin)
    /// </summary>
    [HttpGet("pending-deletion")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetPendingDeletion()
    {
        var (success, message, products) = await _productService.GetPendingDeletionAsync(User);
        
        if (!success)
        {
            return Forbid();
        }

        return Ok(products);
    }

    /// <summary>
    /// Aprova deleção de produto (apenas Admin)
    /// </summary>
    [HttpPost("{id}/approve-deletion")]
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

    /// <summary>
    /// Rejeita deleção de produto (apenas Admin)
    /// </summary>
    [HttpPost("{id}/reject-deletion")]
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

    /// <summary>
    /// Usuário adquire produto
    /// </summary>
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

    /// <summary>
    /// Lista produtos adquiridos pelo usuário logado
    /// </summary>
    [HttpGet("my-products")]
    [Authorize(Policy = "UsuarioOrAbove")]
    public async Task<IActionResult> GetMyProducts()
    {
        var products = await _productService.GetMyProductsAsync(User);
        return Ok(products);
    }
}


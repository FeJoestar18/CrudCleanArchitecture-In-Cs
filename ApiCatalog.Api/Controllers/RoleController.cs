using ApiCatalog.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController(RoleService roleService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "FuncionarioOrAbove")]
    public async Task<IActionResult> GetAll()
    {
        var roles = await roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AddRole([FromBody] AddRoleDto dto)
    {
        var success = await roleService.AddRoleAsync(User, dto.Name, dto.Level, dto.ParentRoleId);
        
        if (!success)
        {
            return Forbid();
        }

        return Ok(new { message = "Role criado com sucesso" });
    }
}

public record AddRoleDto(string Name, int Level, int? ParentRoleId = null);


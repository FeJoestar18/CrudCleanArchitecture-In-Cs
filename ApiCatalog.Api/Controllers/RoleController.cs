using ApiCatalog.Application.Services;
using ApiCatalog.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiCatalog.Application.Common;

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
        return this.OkWithMessage(roles, Messages.JsonResponsesApi.Success);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AddRole([FromBody] AddRoleDto dto)
    {
        var success = await roleService.AddRoleAsync(User, dto.Name, dto.Level, dto.ParentRoleId);
        
        return !success ? this.ForbidWithMessage(Messages.Roles.InsufficientPermissions) : this.CreatedWithMessage<object>(nameof(GetAll), null, null, Messages.Roles.RoleCreated);
    }
}

public record AddRoleDto(string Name, int Level, int? ParentRoleId = null);


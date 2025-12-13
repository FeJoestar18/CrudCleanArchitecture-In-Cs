using ApiCatalog.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ApiCatalog.Application.Policies;

namespace ApiCatalog.Application.Services;

public class RoleService(IRoleRepository roleRepository, IAuthorizationService authorizationService)
{
    private async Task<bool> CanPerformActionAsync(ClaimsPrincipal user, int requiredLevel)
    {
        var authResult = await authorizationService.AuthorizeAsync(
            user, 
            null, 
            new MinimumRoleLevelRequirement(requiredLevel)
        );
        
        return authResult.Succeeded;
    }

    public async Task<List<Domain.Entities.Role>> GetAllRolesAsync()
    {
        return await roleRepository.GetAllAsync();
    }

    public async Task<bool> AddRoleAsync(ClaimsPrincipal user, string name, int level, int? parentRoleId = null)
    {
        if (!await CanPerformActionAsync(user, 3))
        {
            return false;
        }

        var newRole = new Domain.Entities.Role
        {
            Name = name,
            Level = level,
            ParentRoleId = parentRoleId
        };

        await roleRepository.AddAsync(newRole);
        await roleRepository.SaveChangesAsync();
        return true;
    }
}


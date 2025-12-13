using ApiCatalog.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ApiCatalog.Application.Policies;

namespace ApiCatalog.Application.Services;

public class RoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthorizationService _authorizationService;

    public RoleService(IRoleRepository roleRepository, IAuthorizationService authorizationService)
    {
        _roleRepository = roleRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Verifica se o usuário tem nível suficiente para realizar uma ação
    /// </summary>
    public async Task<bool> CanPerformActionAsync(ClaimsPrincipal user, int requiredLevel)
    {
        var authResult = await _authorizationService.AuthorizeAsync(
            user, 
            null, 
            new MinimumRoleLevelRequirement(requiredLevel)
        );
        
        return authResult.Succeeded;
    }

    /// <summary>
    /// Obtém todos os roles disponíveis
    /// </summary>
    public async Task<List<Domain.Entities.Role>> GetAllRolesAsync()
    {
        return await _roleRepository.GetAllAsync();
    }

    /// <summary>
    /// Adiciona um novo role (apenas admins podem fazer isso)
    /// </summary>
    public async Task<bool> AddRoleAsync(ClaimsPrincipal user, string name, int level, int? parentRoleId = null)
    {
        // Validar se o usuário é admin
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

        await _roleRepository.AddAsync(newRole);
        await _roleRepository.SaveChangesAsync();
        return true;
    }
}


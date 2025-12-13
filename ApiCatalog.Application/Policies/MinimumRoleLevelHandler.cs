using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ApiCatalog.Application.Policies;

public class MinimumRoleLevelHandler : AuthorizationHandler<MinimumRoleLevelRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        MinimumRoleLevelRequirement requirement)
    {
        var levelClaim = context.User.FindFirst("role_level")?.Value;
        
        if (int.TryParse(levelClaim, out var userLevel))
        {
            if (userLevel >= requirement.MinimumLevel)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}


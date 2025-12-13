using Microsoft.AspNetCore.Authorization;

namespace ApiCatalog.Application.Policies;

public class MinimumRoleLevelRequirement : IAuthorizationRequirement
{
    public int MinimumLevel { get; }
    
    public MinimumRoleLevelRequirement(int minimumLevel)
    {
        MinimumLevel = minimumLevel;
    }
}


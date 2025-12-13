using Microsoft.AspNetCore.Authorization;

namespace ApiCatalog.Application.Policies;

public class MinimumRoleLevelRequirement(int minimumLevel) : IAuthorizationRequirement
{
    public int MinimumLevel { get; } = minimumLevel;
}


using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using ApiCatalog.Application.Policies;

namespace ApiCatalog.Api.Extensions;

public static class AuthorizationExtensions
{
    public static void AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, MinimumRoleLevelHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy("UsuarioOrAbove", policy =>
                policy.Requirements.Add(new MinimumRoleLevelRequirement(1)))
            .AddPolicy("FuncionarioOrAbove", policy =>
                policy.Requirements.Add(new MinimumRoleLevelRequirement(2)))
            .AddPolicy("AdminOnly", policy =>
                policy.Requirements.Add(new MinimumRoleLevelRequirement(3)));
    }
}


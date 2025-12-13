using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using ApiCatalog.Application.Policies;

namespace ApiCatalog.Api.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, MinimumRoleLevelHandler>();

        services.AddAuthorization(options =>
        {
            // Level 1 = Usuario
            options.AddPolicy("UsuarioOrAbove", policy =>
                policy.Requirements.Add(new MinimumRoleLevelRequirement(1)));
            
            // Level 2 = Funcionario
            options.AddPolicy("FuncionarioOrAbove", policy =>
                policy.Requirements.Add(new MinimumRoleLevelRequirement(2)));

            // Level 3 = Admin
            options.AddPolicy("AdminOnly", policy =>
                policy.Requirements.Add(new MinimumRoleLevelRequirement(3)));
        });

        return services;
    }
}


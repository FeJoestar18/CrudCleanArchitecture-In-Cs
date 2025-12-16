using Microsoft.Extensions.DependencyInjection;
using ApiCatalog.Application.Services;

namespace ApiCatalog.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Application Services
        services.AddScoped<AuthService>();
        services.AddScoped<RoleService>();
        services.AddScoped<ProductService>();

        return services;
    }
}

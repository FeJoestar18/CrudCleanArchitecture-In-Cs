using Microsoft.Extensions.DependencyInjection;
using ApiCatalog.Application.Services;
using ApiCatalog.Application.Services.Interface;
using ApiCatalog.Application.Services.Rules;

namespace ApiCatalog.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<RoleService>();
        services.AddScoped<ProductService>();
        services.AddScoped<IUserValidationService, UserValidationService>();
    }
}
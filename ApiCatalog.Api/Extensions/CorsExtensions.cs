using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCatalog.Api.Extensions;

public static class CorsExtensions
{
    private const string MyCorsPolicy = "MinhaPoliticaCors";
    public static void AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(MyCorsPolicy, builder =>
            {
                var allowed = configuration["Cors:AllowedOrigins"];
                if (!string.IsNullOrWhiteSpace(allowed))
                {
                    var origins = allowed
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .ToArray();

                    builder.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
                else
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });
    }

    public static void UseCorsConfiguration(this IApplicationBuilder app)
    {
        app.UseCors(MyCorsPolicy);
    }
}
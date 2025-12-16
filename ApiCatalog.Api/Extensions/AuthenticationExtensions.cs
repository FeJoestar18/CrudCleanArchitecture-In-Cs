using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiCatalog.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddSmartAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"] 
                  ?? throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
        var issuer = configuration["Jwt:Issuer"] 
                     ?? throw new InvalidOperationException("JWT Issuer is not configured in appsettings.json");
        var audience = configuration["Jwt:Audience"] 
                       ?? throw new InvalidOperationException("JWT Audience is not configured in appsettings.json");

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "SmartScheme";
            options.DefaultAuthenticateScheme = "SmartScheme";
            options.DefaultChallengeScheme = "SmartScheme";
        })
        .AddPolicyScheme("SmartScheme", "JWT or Cookie", options =>
        {
            options.ForwardDefaultSelector = ctx =>
            {
                var authHeader = ctx.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    return JwtBearerDefaults.AuthenticationScheme;
                return "SmartCookie";
            };
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        })
        .AddCookie("SmartCookie", options =>
        {
            options.Cookie.Name = "AuthCookie";
            options.LoginPath = "/api/Auth/login";
            options.LogoutPath = "/api/Auth/logout";
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
        });

        return services;
    }
}

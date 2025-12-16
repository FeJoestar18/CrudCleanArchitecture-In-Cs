using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiCatalog.Api.Extensions;

public static class AuthenticationExtensions
{
    private const string CookieScheme = "SmartCookie";
    private const string PolicyScheme = "SmartScheme";

    public static void AddSmartAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = PolicyScheme;
                options.DefaultAuthenticateScheme = PolicyScheme;
                options.DefaultChallengeScheme = PolicyScheme;
            })
            .AddPolicyScheme(PolicyScheme, "JWT or Cookie", options =>
            {
                options.ForwardDefaultSelector = ctx =>
                {
                    var authHeader = ctx.Request.Headers["Authorization"].ToString();
                    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
                        return JwtBearerDefaults.AuthenticationScheme;
                    return CookieScheme;
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
                };
            })
            .AddCookie(CookieScheme, options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.LoginPath = "/api/Auth/login";
                options.LogoutPath = "/api/Auth/logout";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = System.TimeSpan.FromHours(8);
            });
    }
}

using System.Text.Json;
using System.Threading.RateLimiting;
using ApiCatalog.Application.Common;
using Microsoft.AspNetCore.RateLimiting;

namespace ApiCatalog.Api.Extensions
{
    public static class RateLimitingExtensions
    {
        public static void AddRateLimiterPolicies(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, ct) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    var payload = JsonSerializer.Serialize(new
                    {
                        errorCode = Messages.JsonResponsesApi.ErrorCode,
                        message = Messages.JsonResponsesApi.VeryRequest
                    });
                    await context.HttpContext.Response.WriteAsync(payload, ct);
                };

                options.AddFixedWindowLimiter("Public", opt =>
                {
                    opt.PermitLimit = 1000;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                    opt.AutoReplenishment = true;
                });

                options.AddPolicy("API_Free", httpContext =>
                {
                    var userIdentifier = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                                         ?? httpContext.Connection.RemoteIpAddress?.ToString()
                                         ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(userIdentifier, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromSeconds(30),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
                });

                options.AddPolicy("API_Premium", httpContext =>
                {
                    var userIdentifier = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(userIdentifier, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromSeconds(10),
                        QueueLimit = 2,
                        AutoReplenishment = true
                    });
                });
            });
        }

        // Global Rate Limiter
        // public static IServiceCollection AddRateLimiterGlobal(this IServiceCollection services)
        // {
        //     services.AddRateLimiter(rateLimitOptions =>
        //     {
        //         rateLimitOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        //
        //         rateLimitOptions.OnRejected = async (context, ct) =>
        //         {
        //             context.HttpContext.Response.ContentType = "application/json";
        //             context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        //             var payload = JsonSerializer.Serialize(new
        //             {
        //                 errorCode = Messages.JsonResponsesApi.ErrorCode,
        //                 message = Messages.JsonResponsesApi.VeryRequest
        //             });
        //             await context.HttpContext.Response.WriteAsync(payload, ct);
        //         };
        //
        //         rateLimitOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        //         {
        //             var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        //
        //             return RateLimitPartition.GetFixedWindowLimiter(
        //                 partitionKey: remoteIp,
        //                 factory: _ => new FixedWindowRateLimiterOptions
        //                 {
        //                     AutoReplenishment = true,
        //                     PermitLimit = 10,
        //                     QueueLimit = 2,
        //                     Window = TimeSpan.FromSeconds(5)
        //                 });
        //         });
        //     });
        //
        //     return services;
        // }
    }
}

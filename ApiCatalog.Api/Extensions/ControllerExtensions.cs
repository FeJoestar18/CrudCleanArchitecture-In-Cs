using ApiCatalog.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalog.Api.Extensions;

public static class ControllerExtensions
{
    public static IActionResult OkWithMessage<T>(this ControllerBase controller, T? data, string message)
    {
        var payload = new ApiResponse<T?>(true, message, data);
        return controller.Ok(payload);
    }

    public static IActionResult CreatedWithMessage<T>(this ControllerBase controller, string actionName, object? routeValues, T? data, string message)
    {
        var payload = new ApiResponse<T?>(true, message, data);
        return controller.CreatedAtAction(actionName, routeValues, payload);
    }

    public static IActionResult NoContentWithMessage(this ControllerBase controller, string message)
    {
        controller.Response.Headers["X-Message"] = message;
        return controller.NoContent();
    }

    public static IActionResult BadRequestWithMessage(this ControllerBase controller, string? message)
    {
        var payload = new ApiResponse<object?>(false, message, null);
        return controller.BadRequest(payload);
    }

    public static IActionResult NotFoundWithMessage(this ControllerBase controller, string message)
    {
        var payload = new ApiResponse<object?>(false, message, null);
        return controller.NotFound(payload);
    }

    public static IActionResult ForbidWithMessage(this ControllerBase controller, string message)
    {
        controller.Response.Headers["X-Message"] = message;
        return controller.Forbid();
    }

    public static IActionResult StatusCodeWithMessage(this ControllerBase controller, int statusCode, string message, object? data = null)
    {
        var payload = new ApiResponse<object?>(statusCode is >= 200 and < 300, message, data);
        return controller.StatusCode(statusCode, payload);
    }
}
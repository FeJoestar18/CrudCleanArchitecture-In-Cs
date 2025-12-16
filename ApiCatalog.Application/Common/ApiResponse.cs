namespace ApiCatalog.Application.Common;

public class ApiResponse<T>(bool success, string? message, T? data = default)
{
    public bool Success { get; set; } = success;
    public string? Message { get; set; } = message;
    public T? Data { get; set; } = data;
}
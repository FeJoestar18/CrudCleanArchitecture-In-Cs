namespace ApiCatalog.Application.Common;

public sealed class Result
{
    public bool Success { get; }
    public string? ErrorCode { get; }

    private Result(bool success, string? errorCode)
    {
        Success = success;
        ErrorCode = errorCode;
    }

    public static Result Ok() => new(true, null);

    public static Result Fail(string errorCode) => new(false, errorCode);
}


namespace Infrastructure.Api.Common;

public class ApiException : Exception, IApiException
{
    public int StatusCode { get; }
    public object? Details { get; }

    public ApiException(string message, int statusCode = 400, object? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        Details = details;
    }

    public static ApiException BadRequest(string message, object? details = null)
        => new(message, 400, details);

    public static ApiException NotFound(string message, object? details = null)
        => new(message, 404, details);

    public static ApiException Unauthorized(string message = "Unauthorized", object? details = null)
        => new(message, 401, details);

    public static ApiException Internal(string message = "Internal server error", object? details = null)
        => new(message, 500, details);
}

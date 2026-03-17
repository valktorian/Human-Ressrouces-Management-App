using Infrastructure.Api.Common;
using System.Net;
using System.Text.Json;

namespace Infrastructure.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            LogApiException(context, ex);
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message, ex.Details);
        }
        catch (Exception ex)
        {
            LogUnhandledException(context, ex);
            await HandleExceptionAsync(
                context,
                (int)HttpStatusCode.InternalServerError,
                "Internal server error");
        }
    }

    private void LogApiException(HttpContext context, ApiException ex)
    {
        _logger.LogWarning(
            ex,
            "API route failed. Method: {Method}, Path: {Path}, QueryString: {QueryString}, Endpoint: {Endpoint}, TraceId: {TraceId}, UserId: {UserId}, StatusCode: {StatusCode}, Details: {@Details}",
            context.Request.Method,
            context.Request.Path.Value,
            context.Request.QueryString.Value,
            context.GetEndpoint()?.DisplayName,
            context.TraceIdentifier,
            context.User?.Identity?.Name ?? "anonymous",
            ex.StatusCode,
            ex.Details);
    }

    private void LogUnhandledException(HttpContext context, Exception ex)
    {
        _logger.LogError(
            ex,
            "Unhandled route exception. Method: {Method}, Path: {Path}, QueryString: {QueryString}, Endpoint: {Endpoint}, TraceId: {TraceId}, UserId: {UserId}",
            context.Request.Method,
            context.Request.Path.Value,
            context.Request.QueryString.Value,
            context.GetEndpoint()?.DisplayName,
            context.TraceIdentifier,
            context.User?.Identity?.Name ?? "anonymous");
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message, object? details = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            success = false,
            error = message,
            details,
            statusCode,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

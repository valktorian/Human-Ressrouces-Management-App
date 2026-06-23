namespace Infrastructure.Api.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}

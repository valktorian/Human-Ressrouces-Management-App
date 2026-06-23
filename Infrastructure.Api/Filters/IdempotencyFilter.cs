using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Api.Filters;

public class IdempotencyFilter : IAsyncActionFilter
{
    private readonly IMemoryCache _cache;

    public IdempotencyFilter(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!HttpMethods.IsPost(context.HttpContext.Request.Method)
            && !HttpMethods.IsPut(context.HttpContext.Request.Method)
            && !HttpMethods.IsPatch(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        var key = context.HttpContext.Request.Headers["Idempotency-Key"].ToString();
        if (string.IsNullOrWhiteSpace(key))
        {
            await next();
            return;
        }

        var cacheKey = $"idempotency:{key}";

        if (_cache.TryGetValue(cacheKey, out IdempotencyCacheEntry? cached) && cached is not null)
        {
            context.Result = new ObjectResult(cached.Value) { StatusCode = cached.StatusCode };
            return;
        }

        var executed = await next();

        if (executed.Result is ObjectResult result)
        {
            _cache.Set(cacheKey, new IdempotencyCacheEntry
            {
                Value = result.Value,
                StatusCode = result.StatusCode ?? 200
            }, TimeSpan.FromHours(24));
        }
    }
}

public class IdempotencyCacheEntry
{
    public object? Value { get; set; }
    public int StatusCode { get; set; }
}

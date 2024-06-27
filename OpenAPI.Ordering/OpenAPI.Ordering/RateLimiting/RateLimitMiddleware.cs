using Microsoft.Extensions.Caching.Memory;

namespace OpenAPI.Ordering.RateLimiting
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly int _limit;
        private readonly TimeSpan _period;

        public RateLimitMiddleware(RequestDelegate next, IMemoryCache cache, RateLimitOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _limit = options.Limit;
            _period = options.Period;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var cacheKey = $"{ipAddress}_{context.Request.Path}";

            if (!_cache.TryGetValue(cacheKey, out RateLimitInfo rateLimit))
            {
                rateLimit = new RateLimitInfo { Count = 0, Timestamp = DateTime.UtcNow };
                _cache.Set(cacheKey, rateLimit, _period);
            }

            rateLimit.Count++;

            if (rateLimit.Count > _limit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded.");
                return;
            }

            await _next(context);
        }
    }
}

using EngConnect.Services.Caching.RateLimiting;
using Microsoft.Extensions.Options;
using System.Net;

namespace EngConnect.Api.Middlewares
{
    public sealed class RedisRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimitStore _rateLimitStore;
        private readonly RateLimitingOptions _options;

        public RedisRateLimitingMiddleware(
            RequestDelegate next,
            IRateLimitStore rateLimitStore,
            IOptions<RateLimitingOptions> options)
        {
            _next = next;
            _rateLimitStore = rateLimitStore;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.User.Identity?.IsAuthenticated == true
                ? context.User.Identity!.Name
                : null;

            var clientId = !string.IsNullOrEmpty(userId)
                ? $"user:{userId}"
                : $"ip:{context.Connection.RemoteIpAddress}";

            var window = TimeSpan.FromSeconds(_options.WindowSeconds);
            var key = $"ratelimit:{clientId}";

            long count;
            try
            {
                count = await _rateLimitStore.IncrementAsync(key, window, context.RequestAborted);
            }
            catch
            {
                // Fail-open
                await _next(context);
                return;
            }

            if (count > _options.PermitLimit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";

                var problem = new
                {
                    status = StatusCodes.Status429TooManyRequests,
                    title = "Too Many Requests",
                    detail = "You have exceeded the allowed request rate. Please try again later."
                };

                await context.Response.WriteAsJsonAsync(problem, context.RequestAborted);
                return;
            }

            await _next(context);
        }
    }
}
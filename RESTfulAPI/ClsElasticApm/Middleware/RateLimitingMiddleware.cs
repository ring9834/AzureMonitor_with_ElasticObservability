using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WellingtonWeatherRecommendationApi.Services;

namespace WellingtonWeatherRecommendationApi.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimiterService _rateLimiter;

        public RateLimitingMiddleware(RequestDelegate next, RateLimiterService rateLimiter)
        {
            _next = next;
            _rateLimiter = rateLimiter;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var isAllowed = await _rateLimiter.IsRequestAllowedAsync(clientIp);

            if (!isAllowed)
            {
                var retryAfter = _rateLimiter.GetRetryAfterTime(clientIp);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = retryAfter.HasValue ? ((int)retryAfter.Value.TotalSeconds).ToString() : "300";
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            await _next(context);
        }
    }
}
using Microsoft.Extensions.Options;
using System.Net;
using ColorExtractorApi.Services;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimitService _rateLimitService;
        private readonly RateLimitOptions _options;

        public RateLimitMiddleware(RequestDelegate next, IRateLimitService rateLimitService, IOptions<RateLimitOptions> options)
        {
            _next = next;
            _rateLimitService = rateLimitService;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Equals("/api/auth/login", StringComparison.OrdinalIgnoreCase)
                && context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                if (!_rateLimitService.AllowRequest(ipAddress, _options))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    await context.Response.WriteAsync("Too many login attempts. Please try again later.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
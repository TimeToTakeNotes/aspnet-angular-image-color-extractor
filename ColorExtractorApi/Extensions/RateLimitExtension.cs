// Extension method to encapsulate and modularize rate limiting setup
// Registers memory cache, config binding, and rate limiting service dependencies
// Also adds the custom middleware that enforces rate limits

using ColorExtractorApi.Middleware;
using ColorExtractorApi.Models;
using ColorExtractorApi.Services;
using ColorExtractorApi.Services.Interfaces;

namespace ColorExtractorApi.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration config)
        {
            services.AddMemoryCache();
            services.Configure<RateLimitOptions>(config.GetSection("RateLimiting"));
            services.AddSingleton<IRateLimitService, RateLimitService>();
            return services;
        }

        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
        {
            app.UseMiddleware<RateLimitMiddleware>();
            return app;
        }
    }
}
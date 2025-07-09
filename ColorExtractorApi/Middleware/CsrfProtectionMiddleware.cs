namespace ColorExtractorApi.Middleware
{
    public class CsrfProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public CsrfProtectionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_env.IsDevelopment()) // Only enforce in production
            {
                var method = context.Request.Method;
                if (method == HttpMethods.Post || method == HttpMethods.Put || method == HttpMethods.Delete)
                {
                    if (!context.Request.Headers.ContainsKey("X-XSRF-TOKEN"))
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("Missing CSRF token header.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
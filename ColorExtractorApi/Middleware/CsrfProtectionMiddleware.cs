namespace ColorExtractorApi.Middleware
{
    public class CsrfProtectionMiddleware
    {
        private readonly RequestDelegate _next;

        public CsrfProtectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var isWriteRequest = method == HttpMethods.Post || method == HttpMethods.Put || method == HttpMethods.Delete;

            if (isWriteRequest)
            {
                var csrfCookie = context.Request.Cookies["XSRF-TOKEN"];
                context.Request.Headers.TryGetValue("X-XSRF-TOKEN", out var csrfHeader);

                if (string.IsNullOrEmpty(csrfCookie) || csrfHeader != csrfCookie)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Invalid or missing CSRF token.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
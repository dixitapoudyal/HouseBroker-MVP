namespace HouseBroker.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
                await HandleAsync(context, ex);
            }
        }

        private static Task HandleAsync(HttpContext context, Exception ex)
        {
            // map common exceptions to status codes
            // expand as needed when more exception types come up
            var (statusCode, message) = ex switch
            {
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ex.Message),
                InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message),
                KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Something went wrong.")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(new { error = message });
        }
    }
}

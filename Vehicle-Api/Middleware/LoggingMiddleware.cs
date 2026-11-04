using System.Diagnostics;

namespace Vehicle_Api.Middleware;

public class LoggingMiddleware(ILogger<LoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var requestThreadId = Environment.CurrentManagedThreadId;
        context.TraceIdentifier = $"CurrentThread_{requestThreadId}";
        logger.LogTrace("Executing HTTP {Method} {PATH} threadId:{threadId}", context.Request.Method,context.Request.Path, requestThreadId);
        var sw = new Stopwatch();
        try
        {

            sw.Start();
            await next(context);
        }
        finally
        {

            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            if (elapsed > 500)
            {
                logger.LogWarning("HTTP {Method} {Path} responded {StatusCode} in {Elapsed} ms", context.Request.Method,context.Request.Path,context.Response.StatusCode, elapsed);
            }
            else
                logger.LogTrace("HTTP {Method} {Path} responded {StatusCode} in {Elapsed} ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsed);

        }
    }
}
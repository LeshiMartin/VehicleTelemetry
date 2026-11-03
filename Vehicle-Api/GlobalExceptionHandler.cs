using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Vehicle_Api;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception,
            "An exception happened while executing the method:{request} path:{path},\n Exception message: {Message}",
            httpContext.Request.Method, httpContext.Request.Path, exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server error",
                Detail = "An unhandled exception happened",
            }
        });
    }
}


public class BadHttpRequestExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<BadHttpRequestExceptionHandler> logger): IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badHttpRequestException)
            return false;

        logger.LogError(exception,
            "A BadHttpRequestException happened while executing the method:{request} path:{path},\n Exception message: {Message}",
            httpContext.Request.Method, httpContext.Request.Path, exception.Message);
        httpContext.Response.StatusCode = badHttpRequestException.StatusCode;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = badHttpRequestException,
            ProblemDetails = new ProblemDetails()
            {
                Status = badHttpRequestException.StatusCode,
                Title = "Bad Http Request",
                Detail = "An invalid http request was sent"
            }
        });
    }
}

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
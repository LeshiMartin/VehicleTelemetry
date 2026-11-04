using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Vehicle_Api;

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
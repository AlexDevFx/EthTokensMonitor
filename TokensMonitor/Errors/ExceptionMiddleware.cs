using Microsoft.AspNetCore.Diagnostics;

namespace TokensMonitor.Errors;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            logger.LogError(exception, "Internal API error");
            response.StatusCode = StatusCodes.Status500InternalServerError;
            if (!context.RequestAborted.IsCancellationRequested)
                await response.WriteAsJsonAsync(new ErrorResponse("Internal error"));
        }
    }
}
using Microsoft.AspNetCore.Diagnostics;
using Work_Experience_Search.Exceptions;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Extensions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("Request {Method} {Path} was cancelled by the client.", httpContext.Request.Method, httpContext.Request.Path);
            return true;
        }

        var errorCode = exception switch
        {
            NotFoundException => ErrorCodes.NotFound,
            ConflictException => ErrorCodes.Conflict,
            _ => ErrorCodes.InternalError,
        };

        logger.LogError(exception, "Unhandled exception for {Method} {Path}; responding with {ErrorCode}.",
            httpContext.Request.Method, httpContext.Request.Path, errorCode);

        await httpContext.Response.WriteErrorAsync(errorCode, cancellationToken);
        return true;
    }
}

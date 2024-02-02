using System.Diagnostics;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Middlewares;

public class GlobalExceptionMiddleware(
    ILogger<GlobalExceptionMiddleware> logger,
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Could not process a request on {Machine}. TraceId: {TraceId}",
                Environment.MachineName,
                Activity.Current?.Id);

            switch (ex)
            {
                case ResourceNotFoundException:
                    var id = httpContext.Request.Path.Value!.Split("/").Last();
                    await Results.Problem(
                            title: $"resource with the id '{id}' could not be found",
                            statusCode: StatusCodes.Status404NotFound,
                            extensions: new Dictionary<string, object?>
                            {
                                { "traceId", Activity.Current?.Id }
                            }
                        )
                        .ExecuteAsync(httpContext);
                    break;
                default:
                    await Results.Problem(
                            title: "we made a mistake",
                            statusCode: StatusCodes.Status500InternalServerError,
                            extensions: new Dictionary<string, object?>
                            {
                                { "traceId", Activity.Current?.Id }
                            }
                        )
                        .ExecuteAsync(httpContext);
                    break;
            }
        }
    }
}
using System.Diagnostics;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Could not process a request on {Machine}. TraceId: {TraceId}",
                Environment.MachineName,
                Activity.Current?.Id);

            switch (ex)
            {
                case ResourceNotFoundException:
                    await Results.Problem(
                            title: "you seem to have been made a mistake",
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
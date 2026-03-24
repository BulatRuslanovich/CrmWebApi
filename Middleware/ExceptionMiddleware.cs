using System.Net;
using System.Text.Json;

namespace CrmWebApi.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Необработанное исключение: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (status, message) = ex switch
        {
            KeyNotFoundException        => (HttpStatusCode.NotFound,            ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,        ex.Message),
            InvalidOperationException   => (HttpStatusCode.Conflict,            ex.Message),
            ArgumentException           => (HttpStatusCode.BadRequest,          ex.Message),
            _                           => (HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)status;

        var payload = JsonSerializer.Serialize(new
        {
            status  = (int)status,
            error   = message,
            traceId = context.TraceIdentifier
        });

        return context.Response.WriteAsync(payload);
    }
}
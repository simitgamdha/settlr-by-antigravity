using System.Net;
using System.Text.Json;
using Settlr.Common.Messages;
using Settlr.Common.Response;

namespace Settlr.Web.Middlewares;

/// <summary>
/// A centralized catch-all middleware that sits at the top of the HTTP pipeline.
/// It ensures that any unhandled exception anywhere in the app is caught and 
/// returned to the client as a consistent, structured JSON response.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Wraps the execution of the next middleware in a try-catch block.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the request forward to the next middleware (or controller)
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the full stack trace for developer debugging
            _logger.LogError(ex, "An unhandled exception occurred in the request pipeline.");
            
            // Format and send a polite error response to the client
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Formats the exception into a standard Settlr API Response object.
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Wrap the raw error in our standard Response wrapper so the frontend 
        // can handle it predictably without crashing.
        Response<object> response = Response<object>.Fail(
            Messages.InternalServerError,
            (int)HttpStatusCode.InternalServerError,
            new[] { exception.Message }
        );

        string jsonResponse = JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(jsonResponse);
    }
}

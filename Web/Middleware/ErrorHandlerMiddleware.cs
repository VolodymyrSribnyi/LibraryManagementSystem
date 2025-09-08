using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Web.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
            ////catch (BookNotFoundException ex)
            ////{
            ////    _logger.LogError(ex, $"Book not found. {ex.Message}");
            ////    context.Response.StatusCode = StatusCodes.Status404NotFound;
            ////    context.Response.ContentType = "application/json";
            ////    var errorResponse = new { Message = ex.Message };
            ////    await context.Response.WriteAsJsonAsync(errorResponse);
            ////}
            //catch (AuthorNotFoundException ex)
            //{
            //    _logger.LogError(ex, $"Author not found. {ex.Message}");
            //    context.Response.StatusCode = StatusCodes.Status404NotFound;
            //    context.Response.ContentType = "application/json";
            //    var errorResponse = new { Message = ex.Message };
            //    await context.Response.WriteAsJsonAsync(errorResponse);
            //}
            //catch (ArgumentNullException ex)
            //{
            //    _logger.LogError(ex, $"A required argument was null. {ex.Message}");
            //    context.Response.StatusCode = StatusCodes.Status400BadRequest;
            //    context.Response.ContentType = "application/json";
            //    var errorResponse = new { Message = ex.Message };
            //    await context.Response.WriteAsJsonAsync(errorResponse);
            //}
            //catch (NullReferenceException ex)
            //{
            //    _logger.LogError(ex, $"A null reference occurred. {ex.Message}");
            //    context.Response.StatusCode = StatusCodes.Status400BadRequest;
            //    context.Response.ContentType = "application/json";
            //    var errorResponse = new { Message = "A required object was null." };
            //    await context.Response.WriteAsJsonAsync(errorResponse);
            //}
            //catch (InvalidOperationException ex)
            //{
            //    _logger.LogError(ex, $"An invalid operation occurred. {ex.Message}");
            //    context.Response.StatusCode = StatusCodes.Status400BadRequest;
            //    context.Response.ContentType = "application/json";
            //    var errorResponse = new { Message = ex.Message };
            //    await context.Response.WriteAsJsonAsync(errorResponse);
            //}
            //catch (UnauthorizedAccessException ex)
            //{
            //    _logger.LogWarning(ex, "Unauthorized access attempt.");
            //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    context.Response.ContentType = "application/json";
            //    var errorResponse = new { Message = "Unauthorized access." };
            //    await context.Response.WriteAsJsonAsync(errorResponse);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogCritical(ex, "An unhandled exception occurred.");
            //    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //    context.Response.ContentType = "application/json";
            //    var errorResponse = new
            //    {
            //        Message = ex.Message,
            //        Detail = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment() ? ex.StackTrace : null
            //    };
            //    await context.Response.WriteAsJsonAsync(errorResponse);
            //}
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var (statusCode, message, logLevel) = GetErrorDetails(ex);

            switch (logLevel)
            {
                case LogLevel.Warning:
                    _logger.LogWarning(ex, message);
                    break;
                case LogLevel.Error:
                    _logger.LogError(ex, message);
                    break;
                case LogLevel.Critical:
                    _logger.LogCritical(ex, message);
                    break;
                default:
                    _logger.LogError(ex, message);
                    break;
            }

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = statusCode;

                await HandleError(context, ex, statusCode, message);
            }
        }

        private async Task HandleError(HttpContext context, Exception ex, int statusCode, string message)
        {
            var errorPath = statusCode switch
            {
                404 => "/Home/NotFound",
                401 => "/User/AccessDenied",
                403 => "/Home/Forbidden",
                409 => "/Home/Conflict",
                _ => "/Home/Error"
            };

            context.Response.Redirect(errorPath);
        }

        private (int statusCode, string message, LogLevel logLevel) GetErrorDetails(Exception exception)
        {
            return exception switch
            {
                AuthorNotFoundException ex => (StatusCodes.Status404NotFound, ex.Message, LogLevel.Warning),
                BookNotFoundException ex => (StatusCodes.Status404NotFound, ex.Message, LogLevel.Warning),

                AuthorExistsException ex => (StatusCodes.Status409Conflict, ex.Message, LogLevel.Error),
                UserExistsException ex => (StatusCodes.Status409Conflict, ex.Message, LogLevel.Error),

                ArgumentNullException ex => (StatusCodes.Status400BadRequest,
                "Required parameter is missing", LogLevel.Error),

                ArgumentException ex => (StatusCodes.Status400BadRequest, ex.Message, LogLevel.Error),

                InvalidOperationException ex => (StatusCodes.Status400BadRequest, ex.Message, LogLevel.Error),

                UnauthorizedAccessException ex => (StatusCodes.Status401Unauthorized, ex.Message, LogLevel.Error),

                ValidationException ex => (StatusCodes.Status400BadRequest,
                ex.Message, LogLevel.Warning),

                TimeoutException => (StatusCodes.Status408RequestTimeout,
                    "Request timeout", LogLevel.Warning),

                _ => (StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred", LogLevel.Critical)
            };
        }
    }
}

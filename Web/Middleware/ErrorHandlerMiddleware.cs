using Domain.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Web.Middleware
{
    /// <summary>
    /// Middleware that handles exceptions thrown during the request pipeline, logs the errors,  and redirects the
    /// response to appropriate error pages based on the exception type.
    /// </summary>
    /// <remarks>This middleware intercepts unhandled exceptions, determines the appropriate HTTP status code,
    /// log level, and error message based on the exception type, and redirects the user to a predefined  error page. It
    /// ensures that exceptions are logged with the appropriate severity and that users  receive a user-friendly error
    /// response.  Common exception types handled include: - <see cref="NotFoundException"/>: Redirects to a "Not Found"
    /// page with a 404 status code. - <see cref="UnauthorizedAccessException"/>: Redirects to an "Access Denied" page
    /// with a 401 status code. - <see cref="ValidationException"/>: Logs a warning and redirects to a 400 error page. -
    /// Other exceptions default to a 500 status code and a generic error page.  This middleware should be registered
    /// early in the pipeline to ensure it can catch exceptions  from subsequent middleware components.</remarks>
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
        }

        /// <summary>
        /// Handles exceptions that occur during the processing of an HTTP request by logging the error and preparing an
        /// appropriate HTTP response.
        /// </summary>
        /// <remarks>This method determines the appropriate HTTP status code, log level, and error message
        /// based on the exception, logs the error using the configured logger, and sets the HTTP response status code
        /// and content if the response has not already started.</remarks>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response.</param>
        /// <param name="ex">The <see cref="Exception"/> that was thrown during request processing.</param>
        /// <returns></returns>
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

                HandleError(context, ex, statusCode, message);
            }
        }

        /// <summary>
        /// Handles an error by redirecting the HTTP response to an appropriate error page based on the status code.
        /// </summary>
        /// <remarks>The method redirects the response to a predefined error page based on the provided
        /// <paramref name="statusCode"/>.  Common status codes and their corresponding redirection paths include: <list
        /// type="bullet"> <item><description>404: Redirects to "/Home/NotFound".</description></item>
        /// <item><description>401: Redirects to "/User/AccessDenied".</description></item> <item><description>403:
        /// Redirects to "/Home/Forbidden".</description></item> <item><description>409: Redirects to
        /// "/Home/Conflict".</description></item> <item><description>All other status codes: Redirects to
        /// "/Home/Error".</description></item> </list></remarks>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response.</param>
        /// <param name="ex">The <see cref="Exception"/> that occurred. This parameter is not used in the redirection logic but may be
        /// logged or processed elsewhere.</param>
        /// <param name="statusCode">The HTTP status code representing the type of error. Determines the redirection path.</param>
        /// <param name="message">A custom error message. This parameter is not used in the redirection logic but may be logged or displayed
        /// elsewhere.</param>
        /// <returns></returns>
        private void HandleError(HttpContext context, Exception ex, int statusCode, string message)
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

        /// <summary>
        /// Maps an exception to a corresponding HTTP status code, error message, and log level.
        /// </summary>
        /// <remarks>This method provides a standardized way to interpret exceptions in the context of
        /// HTTP responses and logging. Specific exception types are mapped to predefined status codes, messages, and
        /// log levels.  If the exception type is not explicitly handled, a default response of 500 Internal Server
        /// Error with a critical log level is returned.</remarks>
        /// <param name="exception">The exception to map. Must not be <see langword="null"/>.</param>
        /// <returns>A tuple containing the HTTP status code, error message, and log level associated with the provided
        /// exception.</returns>
        private (int statusCode, string message, LogLevel logLevel) GetErrorDetails(Exception exception)
        {
            return exception switch
            {
                NotFoundException ex => (StatusCodes.Status404NotFound, ex.Message, LogLevel.Warning),

                BadRequestException ex => (StatusCodes.Status400BadRequest, ex.Message, LogLevel.Error),

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

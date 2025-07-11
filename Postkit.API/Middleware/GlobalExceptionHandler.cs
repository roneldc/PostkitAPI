using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Postkit.Shared.Responses;

namespace Postkit.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this.logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "An unhandled exception occurred.");

            if (exception is OperationCanceledException && cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Request was canceled.");
                return false;
            }

            var statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred. Please try again later.";

            if (exception is InvalidOperationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Invalid request. Please check your input and try again.";
            }
            else if (exception is ArgumentNullException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "One or more required parameters are missing.";
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "You do not have permission to access this resource.";
            }

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            var errorResponse = new ApiResponse<object>(
                success: false,
                message: message,
                status: statusCode,
                data: null
            );

            await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }
}
using Microsoft.AspNetCore.Diagnostics;
using Postkit.API.GlobalException;
using Postkit.Shared.Exceptions;
using System.Text.Json;

namespace Postkit.API.Exceptions
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
            var response = exception switch 
            { 
               NotFoundException ex => new ErrorResponse
               {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message
                },
                UnauthorizedException ex => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = ex.Message
                },
                ForbiddenException ex => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = ex.Message
                },
                ValidationException ex => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                },
                ConflictException ex => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = ex.Message
                },
                _ => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred."
                }
            };

            logger.LogError(exception, "Exception handled: {ExceptionType}", exception.GetType().Name);

            httpContext.Response.StatusCode = response.StatusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken);

            return true;
        }
    }
}

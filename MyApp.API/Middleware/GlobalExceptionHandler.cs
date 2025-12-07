using Microsoft.AspNetCore.Diagnostics;
using MyApp.API.Exceptions;
using System.Net;
using System.Text.Json;

namespace MyApp.API.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception.Message, "An unhandled exception occurred");

            var response = httpContext.Response;
            response.ContentType = "application/json";

            var error = new ErrorResponse
            {
                Message = exception.Message,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    error.StatusCode = response.StatusCode;
                    break;

                case BadRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    error.StatusCode = response.StatusCode;
                    break;

                case ConflictException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    error.StatusCode = response.StatusCode;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    error.StatusCode = response.StatusCode;
                    error.Detail = exception.StackTrace;
                    break;
            }

            var json = JsonSerializer.Serialize(error);

            await response.WriteAsync(json, cancellationToken);

            return true; // Indicates the exception was handled
        }

        private record ErrorResponse
        {
            public string Message { get; set; } = null!;
            public string? Detail { get; set; }
            public int StatusCode { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        }
    }
}

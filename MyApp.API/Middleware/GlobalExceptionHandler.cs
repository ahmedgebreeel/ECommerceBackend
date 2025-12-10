using ECommerce.Business.DTOs.Errors;
using ECommerce.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace ECommerce.API.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError("Exception Caught: {message}\n{trace}", exception.Message, exception.StackTrace);

            var response = httpContext.Response;
            response.ContentType = "application/json";

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "An internal server error occurred.";
            string? detail = "";

            switch (exception)
            {
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = exception.Message;
                    detail = "Resource not found.";
                    break;

                case BadRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = exception.Message;
                    detail = "Invalid request data.";
                    break;

                case ConflictException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    message = exception.Message;
                    detail = "Data conflict occurred.";
                    break;

                case UnauthorizedException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    detail = "You are not authorized to perform this action.";
                    break;
            }


            response.StatusCode = statusCode;


            var errorResponse = new ApiErrorResponseDto
            {
                StatusCode = statusCode,
                Message = message,
                Detail = detail
            };

            await response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }
}

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechHub.Domain.Exceptions;

namespace TechHub.Api.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occurred: {Message}, Path: { Path}", exception.Message, httpContext.Request.Path);

            var (status, title) = exception switch
            {
                NotFoundException =>
               (StatusCodes.Status404NotFound, "Not Found"),
                BadRequestException =>
                    (StatusCodes.Status400BadRequest, "Bad Request"),
                _ =>
                    (StatusCodes.Status500InternalServerError, "Server Error")
            };

            var problemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Type = $"https://httpstatuses.com/{status}",
                Instance = httpContext.Request.Path,
                Detail = _env.IsDevelopment() ? exception.Message : "An error occurred while processing your request.",
            };

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

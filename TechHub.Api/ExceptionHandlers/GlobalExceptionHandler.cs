using FluentValidation;
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
            _logger.LogError(exception, "Exception occurred: {Message}, Path: {Path}", exception.Message, httpContext.Request.Path);

            var (status, title, errors) = exception switch
            {
                NotFoundException =>
               (StatusCodes.Status404NotFound, "Not Found", null),
                ValidationException validationException =>
               (StatusCodes.Status400BadRequest, "One or more validation errors occurred.",validationException.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                )),
                BadRequestException =>
                    (StatusCodes.Status400BadRequest, "Bad Request",null),
                _ =>
                    (StatusCodes.Status500InternalServerError, "Server Error",null)
            }; ; ;

            var problemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Type = $"https://httpstatuses.com/{status}",
                Instance = httpContext.Request.Path,
                Detail = _env.IsDevelopment() ? exception.Message : "An error occurred while processing your request.",
            };

            // Add validation errors if present
            if (errors != null)
            {
                problemDetails.Extensions["errors"] = errors;
            }

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

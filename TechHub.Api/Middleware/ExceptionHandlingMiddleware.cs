using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto.Signers;
using System.Threading.Tasks;

namespace TechHub.Api.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await  _next(httpContext);
            }
            catch (Exception ex)
            {
                ExceptionDetails problemDetails;
                problemDetails = new ExceptionDetails(
                    Status: StatusCodes.Status500InternalServerError,
                    Type: "https://httpstatuses.com/500",
                    Title: "Internal Server Error",
                    Detail: ex.Message,
                    Errors: null
                      );

                httpContext.Response.StatusCode = problemDetails.Status;

                await httpContext.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }

    internal sealed record ExceptionDetails(
        int Status,
        string Type,
        string Title,
        string Detail,
        IEnumerable<object>? Errors
        );
}

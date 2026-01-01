using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace TechHub.Application.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
          TRequest request,
          RequestHandlerDelegate<TResponse> next,
          CancellationToken cancellationToken)
        {
            string requestName = request.GetType().Name;

            _logger.LogInformation("Handling {RequestName}", requestName);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next(); // Execute the handler

                _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

           
                return response;
            }
            catch(Exception ex) 
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Error handling {RequestName} after {ElapsedMilliseconds} ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

using DogTrainerTestTask.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;

namespace DogTrainerTestTask.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var (statusCode, message) = MapException(exception);
        var exceptionHandlerFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
        httpContext.Response.StatusCode = statusCode;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Detail = message
            },
            AdditionalMetadata = exceptionHandlerFeature?.Endpoint?.Metadata
        });
    }

    private static (int StatusCode, string Title) MapException(Exception exception)
    {
        return exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, e.Message),
            DomainException e => (StatusCodes.Status409Conflict, e.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };
    }
}
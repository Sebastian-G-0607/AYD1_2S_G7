using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace edu_connect_service.Api.Shared.ErrorHandling;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger, IProblemDetailsService _problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ArgumentException => ((int)HttpStatusCode.BadRequest, "Solicitud incorrecta"),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Recurso no encontrado"),
            _ => ((int)HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        if (statusCode == (int)HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Ocurrió un error no controlado: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Advertencia de negocio: {Message}", exception.Message);
        }

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        var handled = await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        return true; 
    }
}

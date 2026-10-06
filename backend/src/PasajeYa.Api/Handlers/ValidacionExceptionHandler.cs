using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PasajeYa.Application.Exceptions;

namespace PasajeYa.Api.Handlers;

public class ValidacionExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidacionException validacion) return false;

        var problemDetails = new ValidationProblemDetails(validacion.Errores) { Status = StatusCodes.Status400BadRequest };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
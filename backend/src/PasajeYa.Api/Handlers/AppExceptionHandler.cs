using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PasajeYa.Application.Exceptions;

namespace PasajeYa.Api.Handlers;

public class AppExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not AppException appException) return false;

        var problemDetails = appException switch
        {
            ValidacionException validacion => new ValidationProblemDetails(validacion.Errores),
            _ => new ProblemDetails { Detail = appException.Message }
        };

        problemDetails.Status = appException.StatusCode;

        httpContext.Response.StatusCode = appException.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
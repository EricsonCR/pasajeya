using System.Net;

namespace PasajeYa.Application.Exceptions;

public sealed class ConflictoException : AppException
{
    public ConflictoException(string mensaje)
        : base(mensaje, (int)HttpStatusCode.Conflict)
    {
    }
}
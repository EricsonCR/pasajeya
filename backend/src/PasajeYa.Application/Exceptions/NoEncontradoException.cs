using System.Net;

namespace PasajeYa.Application.Exceptions;

public sealed class NoEncontradoException : AppException
{
    public NoEncontradoException(string mensaje) :
        base(mensaje, (int)HttpStatusCode.NotFound)
    { }
}
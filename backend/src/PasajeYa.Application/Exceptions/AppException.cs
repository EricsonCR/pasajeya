namespace PasajeYa.Application.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    
    protected AppException(string mensaje, int statusCode) : base(mensaje)
    {
        StatusCode = statusCode;
    }
}
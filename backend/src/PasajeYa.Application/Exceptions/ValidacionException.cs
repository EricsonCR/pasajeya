namespace PasajeYa.Application.Exceptions;

public sealed class ValidacionException : AppException
{
    public IDictionary<string, string[]> Errores { get; }

    public ValidacionException(string mensaje, IDictionary<string, string[]> errores) : base(mensaje, 400)
    {
        Errores = errores;
    }
}
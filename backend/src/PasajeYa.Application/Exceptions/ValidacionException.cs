namespace PasajeYa.Application.Exceptions;

public sealed class ValidacionException : Exception
{
    public IDictionary<string, string[]> Errores { get; }
    
    public ValidacionException(string mensaje, IDictionary<string, string[]> errores) : base(mensaje)
    {
        Errores = errores;
    }
}
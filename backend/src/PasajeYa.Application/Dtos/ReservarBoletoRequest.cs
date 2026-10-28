namespace PasajeYa.Application.Dtos;

public record ReservarBoletoRequest(
    int ViajeId,
    int NumeroAsiento,
    string Dni,
    string Nombres,
    string Apellidos,
    string Correo,
    string? Celular
);
namespace PasajeYa.Application.Dtos;

public record SolicitudCobro(
    string Token,
    string MedioPago,
    string EmisorId,
    int Cuotas,
    string CorreoPagador,
    decimal Monto,
    string ClaveIdempotencia
);
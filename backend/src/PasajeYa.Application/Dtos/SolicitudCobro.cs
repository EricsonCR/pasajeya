namespace PasajeYa.Application.Dtos;

public record SolicitudCobro(
    int BoletoId,
    string Token,
    string MedioPago,
    int Cuotas,
    string CorreoPagador,
    decimal Monto,
    string ClaveIdempotencia
);
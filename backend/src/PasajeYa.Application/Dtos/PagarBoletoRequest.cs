namespace PasajeYa.Application.Dtos;

public record PagarBoletoRequest(
    string Token,
    string MedioPago,
    string EmisorId,
    int Cuotas,
    string CorreoPagador,
    string ClaveIdempotencia
);
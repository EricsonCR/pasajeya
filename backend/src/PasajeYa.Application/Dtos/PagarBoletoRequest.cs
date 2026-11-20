namespace PasajeYa.Application.Dtos;

public record PagarBoletoRequest(
    string Token,
    string MedioPago,
    int Cuotas,
    string CorreoPagador,
    string ClaveIdempotencia
);
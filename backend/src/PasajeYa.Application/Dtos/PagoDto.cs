using PasajeYa.Domain.Enums;

namespace PasajeYa.Application.Dtos;

public record PagoDto(
    int Id,
    EstadoBoleto Estado,
    string NumeroOperacion,
    DateTime FechaPago,
    decimal Monto
);
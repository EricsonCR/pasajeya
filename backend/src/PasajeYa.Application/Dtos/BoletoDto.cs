using PasajeYa.Domain.Enums;

namespace PasajeYa.Application.Dtos;

public record BoletoDto(
    int Id,
    int ViajeId,
    int NumeroAsiento,
    EstadoBoleto Estado,
    DateTime? ReservadoHasta
);
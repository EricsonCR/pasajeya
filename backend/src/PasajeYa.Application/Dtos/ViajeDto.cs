using PasajeYa.Domain.Enums;

namespace PasajeYa.Application.Dtos;

public record ViajeDto(
    int Id,
    DateTime Salida,
    DateTime Llegada,
    TipoServicio TipoServicio,
    decimal Precio,
    int AsientosDisponibles
);
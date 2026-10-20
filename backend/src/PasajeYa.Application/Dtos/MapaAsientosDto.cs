using PasajeYa.Domain.Enums;

namespace PasajeYa.Application.Dtos;

public record MapaAsientosDto(
    int ViajeId,
    string Origen,
    string Destino,
    DateTime Salida,
    TipoServicio TipoServicio,
    decimal Precio,
    IReadOnlyList<AsientoDto> Asientos
);
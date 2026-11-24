using PasajeYa.Domain.Enums;

namespace PasajeYa.Application.Dtos;

public record DetalleBoletoDto(
    int Id,
    EstadoBoleto Estado,
    string Origen,
    string Destino,
    DateTime Salida,
    int NumeroAsiento,
    decimal Precio,
    string Nombres,
    string Apellidos,
    string Dni,
    DateTime? ReservadoHasta
);
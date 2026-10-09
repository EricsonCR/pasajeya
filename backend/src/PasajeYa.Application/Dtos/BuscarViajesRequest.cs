namespace PasajeYa.Application.Dtos;

public record BuscarViajesRequest(
    int Origen,
    int Destino,
    DateOnly Fecha
);
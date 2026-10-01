using PasajeYa.Domain.Enums;

namespace PasajeYa.Domain.Entities;

public class Bus
{
    public int Id { get; set; }
    public required string Placa { get; set; }
    public TipoServicio TipoServicio { get; set; }
    public int Capacidad { get; set; }
}
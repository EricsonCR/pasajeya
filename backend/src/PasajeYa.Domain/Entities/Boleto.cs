using PasajeYa.Domain.Enums;

namespace PasajeYa.Domain.Entities;

public class Boleto
{
    public int Id { get; set; }
    public int ViajeId { get; set; }
    public int NumeroAsiento { get; set; }
    public EstadoBoleto Estado { get; set; }
    public Viaje? Viaje { get; set; }
}
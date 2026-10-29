using PasajeYa.Domain.Enums;

namespace PasajeYa.Domain.Entities;

public class Boleto
{
    public int Id { get; set; }
    public int ViajeId { get; set; }
    public int NumeroAsiento { get; set; }
    public EstadoBoleto Estado { get; set; }
    public string Dni { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Celular { get; set; }
    public DateTime? ReservadoHasta { get; set; }
    public Viaje? Viaje { get; set; }

    public bool EstadoActivo(DateTime ahora)
    {
        return (Estado == EstadoBoleto.Pagado) || (Estado == EstadoBoleto.Reservado && ReservadoHasta > ahora);
    }
}
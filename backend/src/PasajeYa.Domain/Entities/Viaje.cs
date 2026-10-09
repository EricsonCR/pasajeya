using PasajeYa.Domain.Enums;

namespace PasajeYa.Domain.Entities;

public class Viaje
{
    public int Id { get; set; }
    public int RutaId { get; set; }
    public int BusId { get; set; }
    public DateTime Salida { get; set; }
    public DateTime Llegada { get; set; }
    public decimal Precio { get; set; }
    public Ruta? Ruta { get; set; }
    public Bus? Bus { get; set; }
    public ICollection<Boleto> Boletos { get; set; } = [];

    public int AsientosDisponibles()
    {
        var boletosActivos = Boletos.Count(b => b.Estado == EstadoBoleto.Pagado || b.Estado == EstadoBoleto.Reservado);
        var capacidad = Bus!.Capacidad;

        return capacidad - boletosActivos;
    }
}
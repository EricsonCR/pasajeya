using PasajeYa.Domain.Enums;

namespace PasajeYa.Domain.Entities;

public class Pago
{
    public int Id { get; set; }
    public int BoletoId { get; set; }
    public EstadoPago Estado { get; set; }
    public string NumeroOperacion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public Boleto? Boleto { get; set; }
}
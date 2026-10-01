namespace PasajeYa.Domain.Entities;

public class Ruta
{
    public int Id { get; set; }
    public int OrigenId { get; set; }
    public int DestinoId { get; set; }
    public Ciudad? Origen { get; set; }
    public Ciudad? Destino { get; set; }
}
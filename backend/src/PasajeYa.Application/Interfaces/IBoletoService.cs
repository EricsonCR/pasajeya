using PasajeYa.Application.Dtos;

namespace PasajeYa.Application.Interfaces;

public interface IBoletoService
{
    Task<BoletoDto> ReservarAsync(ReservarBoletoRequest request);
    Task<DetalleBoletoDto> ObtenerAsync(int boletoId);
    Task<PagoDto> PagarAsync(int boletoId, PagarBoletoRequest request);
}
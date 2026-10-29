using PasajeYa.Application.Dtos;

namespace PasajeYa.Application.Interfaces;

public interface IBoletoService
{
    Task<BoletoDto> ReservarAsync(ReservarBoletoRequest request);
}
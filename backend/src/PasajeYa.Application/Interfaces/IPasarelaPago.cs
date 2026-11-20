using PasajeYa.Application.Dtos;

namespace PasajeYa.Application.Interfaces;

public interface IPasarelaPago
{
    Task<ResultadoCobro> CobrarAsync(SolicitudCobro solicitud);
}
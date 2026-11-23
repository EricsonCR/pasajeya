
using PasajeYa.Application.Dtos;
using PasajeYa.Application.Interfaces;

namespace PasajeYa.Infrastructure.Pasarelas;

public class MercadoPagoPasarela : IPasarelaPago
{
    private readonly string _accessToken;

    public MercadoPagoPasarela(string accessToken)
    {
        _accessToken = accessToken;
    }

    public Task<ResultadoCobro> CobrarAsync(SolicitudCobro solicitud)
    {
        throw new NotImplementedException();
    }
}
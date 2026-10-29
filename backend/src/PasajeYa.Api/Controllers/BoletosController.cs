using Microsoft.AspNetCore.Mvc;
using PasajeYa.Application.Dtos;
using PasajeYa.Application.Interfaces;

namespace PasajeYa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoletosController : ControllerBase
{
    private readonly IBoletoService _boletoService;

    public BoletosController(IBoletoService boletoService)
    {
        _boletoService = boletoService;
    }

    [HttpPost]
    public async Task<ActionResult<BoletoDto>> ReservarAsiento([FromBody] ReservarBoletoRequest request)
    {
        var boleto = await _boletoService.ReservarAsync(request);
        return StatusCode(StatusCodes.Status201Created, boleto);
    }
}
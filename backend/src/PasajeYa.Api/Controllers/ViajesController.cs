using Microsoft.AspNetCore.Mvc;
using PasajeYa.Application.Dtos;
using PasajeYa.Application.Interfaces;

namespace PasajeYa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ViajesController : ControllerBase
{
    private readonly IViajeService _viajeService;

    public ViajesController(IViajeService viajeService)
    {
        _viajeService = viajeService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ViajeDto>>> BuscarViajes([FromQuery] BuscarViajesRequest request)
    {
        var viajesDto = await _viajeService.BuscarAsync(request);
        return Ok(viajesDto);
    }
}
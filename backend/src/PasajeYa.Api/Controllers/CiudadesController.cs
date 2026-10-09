using Microsoft.AspNetCore.Mvc;
using PasajeYa.Application.Dtos;
using PasajeYa.Application.Interfaces;

namespace PasajeYa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CiudadesController : ControllerBase
{
    private readonly ICiudadService _ciudadService;

    public CiudadesController(ICiudadService ciudadService)
    {
        _ciudadService = ciudadService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CiudadDto>>> GetAll()
    {
        var ciudades = await _ciudadService.GetAllAsync();
        return Ok(ciudades);
    }
}
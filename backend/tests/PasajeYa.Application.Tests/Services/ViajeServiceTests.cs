using NSubstitute;
using PasajeYa.Application.Dtos;
using PasajeYa.Application.Exceptions;
using PasajeYa.Application.Interfaces;
using PasajeYa.Application.Services;
using PasajeYa.Domain.Entities;
using PasajeYa.Domain.Enums;

namespace PasajeYa.Application.Tests.Services;

public class ViajeServiceTests
{
    private readonly IViajeRepository _viajeRepository = Substitute.For<IViajeRepository>();
    private readonly ICiudadRepository _ciudadRepository = Substitute.For<ICiudadRepository>();
    private readonly ViajeService _service;

    public ViajeServiceTests()
    {
        _ciudadRepository.ExisteAsync(Arg.Any<int>()).Returns(true); // por defecto, toda ciudad existe
        _service = new ViajeService(_viajeRepository, _ciudadRepository);
    }

    [Fact]
    public async Task BuscarAsync_OrigenIgualDestino_LanzaErrorEnDestino()
    {
        // Arrange
        var request = new BuscarViajesRequest(1, 1, DateOnly.FromDateTime(DateTime.Today));

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _service.BuscarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("destino"));
    }

    [Fact]
    public async Task BuscarAsync_FechaPasada_LanzaErrorEnFecha()
    {
        // Arrange
        var request = new BuscarViajesRequest(1, 2, DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _service.BuscarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("fecha"));
    }

    [Fact]
    public async Task BuscarAsync_FechaMas30Dias_LanzaErrorEnFecha()
    {
        // Arrange
        var request = new BuscarViajesRequest(1, 2, DateOnly.FromDateTime(DateTime.Today.AddDays(31)));

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _service.BuscarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("fecha"));
    }

    [Fact]
    public async Task BuscarAsync_OrigenNoExiste_LanzaErrorEnOrigen()
    {
        // Arrange
        var request = new BuscarViajesRequest(99, 2, DateOnly.FromDateTime(DateTime.Today));
        _ciudadRepository.ExisteAsync(99).Returns(false);

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _service.BuscarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("origen"));
    }

    [Fact]
    public async Task BuscarAsync_RequestValido_DevuelveViajesComoDto()
    {
        // Arrange
        var viaje = new Viaje
        {
            Id = 1,
            Salida = DateTime.Today.AddDays(1).AddHours(6),
            Llegada = DateTime.Today.AddDays(1).AddHours(14),
            Precio = 69.90m,
            Bus = new Bus { Placa = "ABC123", Capacidad = 10, TipoServicio = TipoServicio.Vip },
            Boletos = [
                new Boleto { Estado = EstadoBoleto.Pagado },
                new Boleto { Estado = EstadoBoleto.Reservado },
                new Boleto { Estado = EstadoBoleto.Anulado }
            ]
        };
        _viajeRepository
            .BuscarAsync(1, 2, Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns([viaje]);

        var request = new BuscarViajesRequest(1, 2, DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        // Act
        var resultado = await _service.BuscarAsync(request);

        // Assert
        var viajeDto = Assert.Single(resultado);
        Assert.Equal(1, viajeDto.Id);
        Assert.Equal(69.9m, viajeDto.Precio);
        Assert.Equal(TipoServicio.Vip, viajeDto.TipoServicio);
        Assert.Equal(8, viajeDto.AsientosDisponibles);

    }
}
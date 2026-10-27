using Microsoft.Extensions.Time.Testing;
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
    private readonly FakeTimeProvider _reloj = new(new DateTimeOffset(2026, 10, 9, 15, 0, 0, TimeSpan.Zero)); // "hoy" = 9 oct, 15:00
    private readonly DateOnly _hoy = new(2026, 10, 9);

    public ViajeServiceTests()
    {
        _ciudadRepository.ExisteAsync(Arg.Any<int>()).Returns(true); // por defecto, toda ciudad existe
        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns((Viaje?)null);
        _service = new ViajeService(_viajeRepository, _ciudadRepository, _reloj);
    }

    [Fact]
    public async Task BuscarAsync_OrigenIgualDestino_LanzaErrorEnDestino()
    {
        // Arrange
        var request = new BuscarViajesRequest(1, 1, _hoy);

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _service.BuscarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("destino"));
    }

    [Fact]
    public async Task BuscarAsync_FechaPasada_LanzaErrorEnFecha()
    {
        // Arrange
        var request = new BuscarViajesRequest(1, 2, _hoy.AddDays(-1));

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _service.BuscarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("fecha"));
    }

    [Fact]
    public async Task BuscarAsync_FechaMas30Dias_LanzaErrorEnFecha()
    {
        // Arrange
        var request = new BuscarViajesRequest(1, 2, _hoy.AddDays(31));

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _service.BuscarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("fecha"));
    }

    [Fact]
    public async Task BuscarAsync_OrigenNoExiste_LanzaErrorEnOrigen()
    {
        // Arrange
        var request = new BuscarViajesRequest(99, 2, _hoy);
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
            Salida = _hoy.AddDays(1).ToDateTime(new TimeOnly(6, 0)),
            Llegada = _hoy.AddDays(1).ToDateTime(new TimeOnly(14, 0)),
            Precio = 69.90m,
            Bus = new Bus { Placa = "ABC123", Capacidad = 10, TipoServicio = TipoServicio.Vip },
            Boletos = [
                new Boleto { Estado = EstadoBoleto.Pagado },
                new Boleto { Estado = EstadoBoleto.Reservado, ReservadoHasta = _reloj.GetLocalNow().DateTime.AddMinutes(10) },
                new Boleto { Estado = EstadoBoleto.Anulado }
            ]
        };
        _viajeRepository
            .BuscarAsync(1, 2, Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns([viaje]);

        var request = new BuscarViajesRequest(1, 2, _hoy.AddDays(1));

        // Act
        var resultado = await _service.BuscarAsync(request);

        // Assert
        var viajeDto = Assert.Single(resultado);
        Assert.Equal(1, viajeDto.Id);
        Assert.Equal(69.9m, viajeDto.Precio);
        Assert.Equal(TipoServicio.Vip, viajeDto.TipoServicio);
        Assert.Equal(8, viajeDto.AsientosDisponibles);

    }

    [Fact]
    public async Task BuscarAsync_FechaHoy_BuscaDesdeLaHoraActual()
    {
        // Arrange
        var request = new BuscarViajesRequest(1, 2, _hoy);

        // Act
        await _service.BuscarAsync(request);

        // Assert
        await _viajeRepository.Received(1).BuscarAsync(
            1, 2,
            new DateTime(2026, 10, 9, 15, 0, 0),  // desde = la hora del reloj falso
            new DateTime(2026, 10, 10));          // hasta = fin del día
    }

    [Fact]
    public async Task BuscarAsync_FechaFutura_BuscaDesdeInicioDelDia()
    {
        // Arrange
        var request = new BuscarViajesRequest(1, 2, _hoy.AddDays(1));

        // Act
        await _service.BuscarAsync(request);

        // Assert
        await _viajeRepository.Received(1).BuscarAsync(
            1, 2,
            new DateTime(2026, 10, 10),  // desde = mañna
            new DateTime(2026, 10, 11)); // hasta = fin del día
    }

    [Fact]
    public async Task ObtenerMapaAsientosAsync_ViajeNoExiste_LanzaNoEncontradoException()
    {
        // Arrange
        int viajeId = 99;

        // Act
        var ex = await Assert.ThrowsAsync<NoEncontradoException>(() => _service.ObtenerMapaAsientosAsync(viajeId));

        // Assert
        Assert.Equal($"El viaje {viajeId} no existe", ex.Message);
    }

    [Fact]
    public async Task ObtenerMapaAsientosAsync_ViajeExiste_DevuelveMapaConAsientos()
    {
        // Arrange
        var viaje = new Viaje
        {
            Id = 1,
            Ruta = new Ruta
            {
                Origen = new Ciudad { Nombre = "Lima" },
                Destino = new Ciudad { Nombre = "Arequipa" }
            },
            Salida = new DateTime(2026, 10, 21, 15, 0, 0),
            Bus = new Bus { Placa = "BDS275", Capacidad = 4, TipoServicio = TipoServicio.Ejecutivo },
            Precio = 179.9m,
            Boletos = [
                new Boleto { NumeroAsiento = 1, Estado = EstadoBoleto.Anulado },
                new Boleto { NumeroAsiento = 2, Estado = EstadoBoleto.Pagado },
                new Boleto { NumeroAsiento = 3, Estado = EstadoBoleto.Reservado, ReservadoHasta = _reloj.GetLocalNow().DateTime.AddMinutes(10) },
            ]
        };

        _viajeRepository.ObtenerPorIdAsync(1).Returns(viaje);

        // Act
        var resultado = await _service.ObtenerMapaAsientosAsync(1);

        // Assert
        Assert.Equal(1, resultado.ViajeId);
        Assert.Equal("Lima", resultado.Origen);
        Assert.Equal("Arequipa", resultado.Destino);
        Assert.Equal(new DateTime(2026, 10, 21, 15, 0, 0), resultado.Salida);
        Assert.Equal(TipoServicio.Ejecutivo, resultado.TipoServicio);
        Assert.Equal(179.9m, resultado.Precio);
        Assert.Equal(4, resultado.Asientos.Count);
        Assert.Equal(1, resultado.Asientos[0].Numero);
        Assert.Equal(2, resultado.Asientos.Count(a => a.Ocupado == true));
        Assert.Equal(2, resultado.Asientos.Count(a => a.Ocupado == false));
    }
}
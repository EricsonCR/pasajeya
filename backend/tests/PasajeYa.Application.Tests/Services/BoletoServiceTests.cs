using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using PasajeYa.Application.Dtos;
using PasajeYa.Application.Exceptions;
using PasajeYa.Application.Interfaces;
using PasajeYa.Application.Services;
using PasajeYa.Domain.Entities;
using PasajeYa.Domain.Enums;

namespace PasajeYa.Application.Tests.Services;

public class BoletoServiceTests
{
    private readonly BoletoService _boletoService;
    private readonly IViajeRepository _viajeRepository = Substitute.For<IViajeRepository>();
    private readonly IBoletoRepository _boletoRepository = Substitute.For<IBoletoRepository>();
    private readonly FakeTimeProvider _reloj = new(new DateTimeOffset(2026, 10, 28, 9, 15, 0, TimeSpan.Zero));
    private readonly ReservarBoletoRequest _requestValido = new(1, 1, "46348500", "Ericson", "Cruz", "ericson@gmail.com", "987654321");


    public BoletoServiceTests()
    {
        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns((Viaje?)null);
        _boletoService = new BoletoService(_boletoRepository, _viajeRepository, _reloj);
    }

    [Fact]
    public async Task ReservarAsync_DniInvalido_LanzaErrorEnDni()
    {
        // Arrange
        var viaje = new Viaje
        {
            Id = 1,
            Bus = new Bus { Placa = "BDS275", Capacidad = 32 }
        };
        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns(viaje);
        var request = _requestValido with { Dni = "4634850" };

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _boletoService.ReservarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("dni"));
    }

    [Fact]
    public async Task ReservarAsync_CorreoInvalido_LanzaErrorEnCorreo()
    {
        // Arrange
        var viaje = new Viaje
        {
            Id = 1,
            Bus = new Bus { Placa = "BDS275", Capacidad = 32 }
        };
        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns(viaje);
        var request = _requestValido with { Correo = "ericson_gmail.com" };

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _boletoService.ReservarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("correo"));
    }

    [Fact]
    public async Task ReservarAsync_ViajeNoExiste_LanzaNoEncontrado()
    {
        // Arrange
        var request = _requestValido with { ViajeId = 0 };

        // Act
        var ex = await Assert.ThrowsAsync<NoEncontradoException>(() => _boletoService.ReservarAsync(request));

        // Assert
        Assert.Equal($"El viaje {request.ViajeId} no existe", ex.Message);
    }

    [Fact]
    public async Task ReservarAsync_AsientoFueraDeRango_LanzaErrorEnNumeroAsiento()
    {
        // Arrange
        var viaje = new Viaje
        {
            Id = 1,
            Bus = new Bus { Placa = "BDS275", Capacidad = 32 }
        };
        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns(viaje);
        var request = _requestValido with { NumeroAsiento = 0 };

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _boletoService.ReservarAsync(request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("numeroAsiento"));
    }

    [Fact]
    public async Task ReservarAsync_AsientoOcupado_LanzaConflicto()
    {
        // Arrange
        var viaje = new Viaje
        {
            Id = 1,
            Bus = new Bus { Placa = "BDS275", Capacidad = 32 },
            Boletos = [
                new Boleto { NumeroAsiento = 1, Estado = EstadoBoleto.Reservado, ReservadoHasta = _reloj.GetLocalNow().DateTime.AddMinutes(15) }
            ]
        };
        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns(viaje);

        // Act
        var ex = await Assert.ThrowsAsync<ConflictoException>(() => _boletoService.ReservarAsync(_requestValido));

        // Assert
        Assert.Equal($"El número asiento {_requestValido.NumeroAsiento} ya está ocupado", ex.Message);
    }

    [Fact]
    public async Task ReservarAsync_DatosValidos_DevuelveBoletoReservado()
    {
        // Arrange
        var viaje = new Viaje
        {
            Id = 1,
            Bus = new Bus { Placa = "BDS275", Capacidad = 32 },
        };
        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns(viaje);

        // Act
        var resultado = await _boletoService.ReservarAsync(_requestValido);

        // Assert
        Assert.Equal(1, resultado.ViajeId);
        Assert.Equal(1, resultado.NumeroAsiento);
        Assert.Equal(EstadoBoleto.Reservado, resultado.Estado);
        Assert.Equal(_reloj.GetLocalNow().DateTime.AddMinutes(15), resultado.ReservadoHasta);
    }

    [Fact]
    public async Task ReservarAsync_ConReservaVencida_EnviaVencidoAlRepositorio()
    {
        // Arrange
        var boletoVencido = new Boleto { NumeroAsiento = 1, Estado = EstadoBoleto.Reservado, ReservadoHasta = _reloj.GetLocalNow().DateTime.AddMinutes(-1) };
        var viaje = new Viaje
        {
            Id = 1,
            Bus = new Bus { Placa = "BDS275", Capacidad = 32 },
            Boletos = [boletoVencido]
        };

        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns(viaje);

        // Act
        await _boletoService.ReservarAsync(_requestValido);

        // Assert
        await _boletoRepository.Received(1).ReservarAsync(Arg.Any<Boleto>(), boletoVencido);
    }
}
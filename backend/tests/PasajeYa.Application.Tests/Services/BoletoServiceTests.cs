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
    private readonly IPasarelaPago _pasarelaPago = Substitute.For<IPasarelaPago>();
    private readonly FakeTimeProvider _reloj = new(new DateTimeOffset(2026, 10, 28, 9, 15, 0, TimeSpan.Zero));
    private readonly ReservarBoletoRequest _requestValido = new(1, 1, "46348500", "Ericson", "Cruz", "ericson@gmail.com", "987654321");
    private readonly PagarBoletoRequest _pagoValido = new("tok_123", "master", 1, "comprador@testuser.com", "clave-1");
    private DateTime Ahora => _reloj.GetLocalNow().DateTime;

    public BoletoServiceTests()
    {
        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns((Viaje?)null);
        _boletoService = new BoletoService(_boletoRepository, _viajeRepository, _reloj, _pasarelaPago);
    }

    [Fact]
    public async Task ReservarAsync_DniInvalido_LanzaErrorEnDni()
    {
        // Arrange
        var viaje = new Viaje
        {
            Id = 1,
            Salida = Ahora.AddDays(1),
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
            Salida = Ahora.AddDays(1),
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
            Salida = Ahora.AddDays(1),
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
            Salida = Ahora.AddDays(1),
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
            Salida = Ahora.AddDays(1),
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
            Salida = Ahora.AddDays(1),
            Bus = new Bus { Placa = "BDS275", Capacidad = 32 },
            Boletos = [boletoVencido]
        };

        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns(viaje);

        // Act
        await _boletoService.ReservarAsync(_requestValido);

        // Assert
        await _boletoRepository.Received(1).ReservarAsync(Arg.Any<Boleto>(), boletoVencido);
    }

    [Theory]
    [InlineData(-60)] // salió hace 1 hora
    [InlineData(0)]   // sale justo ahora
    public async Task ReservarAsync_ViajeYaSalio_LanzaErrorEnViajeId(int minutosDesdeAhora)
    {
        // Arrange
        var viaje = new Viaje
        {
            Id = 1,
            Salida = Ahora.AddMinutes(minutosDesdeAhora),
            Bus = new Bus { Placa = "BDS275", Capacidad = 32 }
        };
        _viajeRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns(viaje);

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _boletoService.ReservarAsync(_requestValido));

        // Assert
        Assert.Equal(["El viaje ya salió, no se puede reservar."], ex.Errores["viajeId"]);
        await _boletoRepository.DidNotReceive().ReservarAsync(Arg.Any<Boleto>(), Arg.Any<Boleto?>());
    }

    [Fact]
    public async Task ObtenerAsync_BoletoNoExiste_LanzaNoEncontrado()
    {
        // Arrange
        _boletoRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns((Boleto?)null);

        // Act
        var ex = await Assert.ThrowsAsync<NoEncontradoException>(() => _boletoService.ObtenerAsync(99999));

        // Assert
        Assert.Equal("El boleto 99999 no existe", ex.Message);
    }

    [Fact]
    public async Task ObtenerAsync_BoletoExiste_DevuelveDetalleConDatosDelViaje()
    {
        // Arrange
        var boleto = new Boleto
        {
            Id = 120,
            NumeroAsiento = 5,
            Estado = EstadoBoleto.Reservado,
            Dni = "46348500",
            Nombres = "Ericson",
            Apellidos = "Cruz",
            ReservadoHasta = Ahora.AddMinutes(15),
            Viaje = new Viaje
            {
                Salida = Ahora.AddDays(1),
                Precio = 69.90m,
                Ruta = new Ruta
                {
                    Origen = new Ciudad { Nombre = "Lima" },
                    Destino = new Ciudad { Nombre = "Arequipa" }
                }
            }
        };
        _boletoRepository.ObtenerPorIdAsync(120).Returns(boleto);

        // Act
        var detalle = await _boletoService.ObtenerAsync(120);

        // Assert
        Assert.Equal(120, detalle.Id);
        Assert.Equal(EstadoBoleto.Reservado, detalle.Estado);
        Assert.Equal("Lima", detalle.Origen);
        Assert.Equal("Arequipa", detalle.Destino);
        Assert.Equal(Ahora.AddDays(1), detalle.Salida);
        Assert.Equal(5, detalle.NumeroAsiento);
        Assert.Equal(69.90m, detalle.Precio);
        Assert.Equal("46348500", detalle.Dni);
        Assert.Equal(Ahora.AddMinutes(15), detalle.ReservadoHasta);
    }

    // ===== PagarAsync (HU-04) =====

    private Boleto BoletoReservado() => new()
    {
        Id = 120,
        NumeroAsiento = 5,
        Estado = EstadoBoleto.Reservado,
        ReservadoHasta = Ahora.AddMinutes(10),
        Viaje = new Viaje { Salida = Ahora.AddDays(1), Precio = 69.90m }
    };

    [Fact]
    public async Task PagarAsync_TokenVacio_LanzaErrorEnToken()
    {
        // Arrange
        _boletoRepository.ObtenerPorIdAsync(120).Returns(BoletoReservado());
        var request = _pagoValido with { Token = "" };

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _boletoService.PagarAsync(120, request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("token"));
    }

    [Fact]
    public async Task PagarAsync_CuotasCero_LanzaErrorEnCuotas()
    {
        // Arrange
        _boletoRepository.ObtenerPorIdAsync(120).Returns(BoletoReservado());
        var request = _pagoValido with { Cuotas = 0 };

        // Act
        var ex = await Assert.ThrowsAsync<ValidacionException>(() => _boletoService.PagarAsync(120, request));

        // Assert
        Assert.True(ex.Errores.ContainsKey("cuotas"));
    }

    [Fact]
    public async Task PagarAsync_BoletoNoExiste_LanzaNoEncontrado()
    {
        // Arrange
        _boletoRepository.ObtenerPorIdAsync(Arg.Any<int>()).Returns((Boleto?)null);

        // Act
        var ex = await Assert.ThrowsAsync<NoEncontradoException>(() => _boletoService.PagarAsync(99999, _pagoValido));

        // Assert
        Assert.Equal("El boleto 99999 no existe", ex.Message);
        await _pasarelaPago.DidNotReceive().CobrarAsync(Arg.Any<SolicitudCobro>());
    }

    [Fact]
    public async Task PagarAsync_DatosValidos_CobraElPrecioDelViajeConLaClaveDelFront()
    {
        // Arrange
        _boletoRepository.ObtenerPorIdAsync(120).Returns(BoletoReservado());
        _pasarelaPago.CobrarAsync(Arg.Any<SolicitudCobro>()).Returns(new ResultadoCobro(true, "PAY01"));

        // Act
        await _boletoService.PagarAsync(120, _pagoValido);

        // Assert
        await _pasarelaPago.Received(1).CobrarAsync(Arg.Is<SolicitudCobro>(s =>
            s.Monto == 69.90m && s.Token == "tok_123" && s.ClaveIdempotencia == "clave-1"));
    }

    [Fact]
    public async Task PagarAsync_PagoAprobado_RegistraPagoYDevuelvePagado()
    {
        // Arrange
        var boleto = BoletoReservado();
        _boletoRepository.ObtenerPorIdAsync(120).Returns(boleto);
        _pasarelaPago.CobrarAsync(Arg.Any<SolicitudCobro>()).Returns(new ResultadoCobro(true, "PAY01"));

        // Act
        var resultado = await _boletoService.PagarAsync(120, _pagoValido);

        // Assert
        await _boletoRepository.Received(1).RegistrarPagoAsync(boleto, Arg.Is<Pago>(p =>
            p.BoletoId == 120 && p.Estado == EstadoPago.Aprobado && p.NumeroOperacion == "PAY01"
            && p.Monto == 69.90m && p.Fecha == Ahora));
        Assert.Equal(120, resultado.Id);
        Assert.Equal(EstadoBoleto.Pagado, resultado.Estado);
        Assert.Equal("PAY01", resultado.NumeroOperacion);
        Assert.Equal(Ahora, resultado.FechaPago);
        Assert.Equal(69.90m, resultado.Monto);
    }

    [Fact]
    public async Task PagarAsync_PagoRechazado_NoRegistraPago()
    {
        // Arrange
        _boletoRepository.ObtenerPorIdAsync(120).Returns(BoletoReservado());
        _pasarelaPago.CobrarAsync(Arg.Any<SolicitudCobro>()).Returns(new ResultadoCobro(false, "PAY02"));

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => _boletoService.PagarAsync(120, _pagoValido));

        // Assert
        await _boletoRepository.DidNotReceive().RegistrarPagoAsync(Arg.Any<Boleto>(), Arg.Any<Pago>());
    }
}
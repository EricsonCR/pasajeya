using PasajeYa.Domain.Entities;
using PasajeYa.Domain.Enums;

namespace PasajeYa.Domain.Tests.Entities;

public class ViajeTests
{
    [Fact]
    public void AsientosDisponibles_SinBoletos_DevuelveCapacidad()
    {
        // Arrange
        var viaje = new Viaje { Bus = new Bus { Placa = "BDS275", Capacidad = 32 } };

        // Act
        var resultado = viaje.AsientosDisponibles();

        // Assert
        Assert.Equal(32, resultado);
    }

    [Fact]
    public void AsientosDisponibles_ConBoletosPagados_RestaUnAsiento()
    {
        // Arrange
        var viaje = new Viaje { Bus = new Bus { Placa = "BDS275", Capacidad = 32 } };
        viaje.Boletos.Add(new Boleto { Estado = EstadoBoleto.Pagado });

        // Act
        var resultado = viaje.AsientosDisponibles();

        // Assert
        Assert.Equal(31, resultado);
    }

    [Fact]
    public void AsientosDisponibles_ConBoletosReservados_RestaUnAsiento()
    {
        // Arrange
        var viaje = new Viaje { Bus = new Bus { Placa = "BDS275", Capacidad = 32 } };
        viaje.Boletos.Add(new Boleto { Estado = EstadoBoleto.Reservado });

        // Act
        var resultado = viaje.AsientosDisponibles();

        // Assert
        Assert.Equal(31, resultado);
    }

    [Fact]
    public void AsientosDisponibles_ConBoletosAnulados_DevuelveCapacidad()
    {
        // Arrange
        var viaje = new Viaje { Bus = new Bus { Placa = "BDS275", Capacidad = 32 } };
        viaje.Boletos.Add(new Boleto { Estado = EstadoBoleto.Anulado });

        // Act
        var resultado = viaje.AsientosDisponibles();

        // Assert
        Assert.Equal(32, resultado);
    }

    [Fact]
    public void AsientosDisponibles_ConTodosBoletosPagados_DevuelveCero()
    {
        // Arrange
        var viaje = new Viaje
        {
            Bus = new Bus { Placa = "BDS275", Capacidad = 2 },
            Boletos = [
                new Boleto { Estado = EstadoBoleto.Pagado},
                new Boleto { Estado = EstadoBoleto.Pagado}
            ]
        };

        // Act
        var resultado = viaje.AsientosDisponibles();

        // Assert
        Assert.Equal(0, resultado);
    }
}
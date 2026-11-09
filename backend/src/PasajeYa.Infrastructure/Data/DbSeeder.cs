using Microsoft.EntityFrameworkCore;
using PasajeYa.Domain.Entities;
using PasajeYa.Domain.Enums;

namespace PasajeYa.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(PasajeYaDbContext context)
    {
        if (!await context.Viajes.AnyAsync(v => v.Salida >= DateTime.Today))
        {
            await CrearViajesAsync(context);
            await CrearBoletosDePruebaAsync(context);
        }
        await SimularVentasAsync(context);
    }

    private static async Task CrearViajesAsync(PasajeYaDbContext context)
    {
        var rutas = await context.Rutas.ToListAsync();
        var buses = await context.Buses.ToListAsync();
        var viajes = new List<Viaje>();
        int[] horas = { 6, 14, 22 };
        int b = 0; // posición del bus en la lista

        foreach (var ruta in rutas)
        {
            for (int dia = 0; dia < 2; dia++)
            {
                foreach (var hora in horas)
                {
                    var bus = buses[b % buses.Count]; // % vuelve a 0 al llegar al final
                    b++;

                    var salida = DateTime.Today.AddDays(dia).AddHours(hora);
                    viajes.Add(new Viaje
                    {
                        RutaId = ruta.Id,
                        BusId = bus.Id,
                        Salida = salida,
                        Llegada = salida.AddHours(8),
                        Precio = 69.90m
                    });
                }
            }
        }

        context.Viajes.AddRange(viajes);
        await context.SaveChangesAsync();
    }

    private static async Task CrearBoletosDePruebaAsync(PasajeYaDbContext context)
    {
        var viajes = await context.Viajes
            .Include(v => v.Bus)
            .Where(v => v.Ruta!.Origen!.Nombre == "Lima"
                && v.Ruta!.Destino!.Nombre == "Arequipa"
                && v.Salida.Date == DateTime.Today)
            .ToListAsync();

        var lleno = viajes.Single(v => v.Salida.Hour == 14);
        var mixto = viajes.Single(v => v.Salida.Hour == 22);
        var boletos = new List<Boleto>();

        for (int asiento = 1; asiento <= lleno.Bus!.Capacidad; asiento++)
        {
            boletos.Add(NuevoBoleto(lleno, asiento, EstadoBoleto.Pagado));
        }

        for (int asiento = 1; asiento <= 4; asiento++)
        {
            boletos.Add(NuevoBoleto(mixto, asiento, EstadoBoleto.Pagado));
        }

        for (int asiento = 5; asiento <= 6; asiento++)
        {
            var reservado = NuevoBoleto(mixto, asiento, EstadoBoleto.Reservado);
            reservado.ReservadoHasta = DateTime.Today.AddDays(1);
            boletos.Add(reservado);
        }

        for (int asiento = 7; asiento <= 8; asiento++)
        {
            boletos.Add(NuevoBoleto(mixto, asiento, EstadoBoleto.Anulado));
        }

        context.Boletos.AddRange(boletos);
        await context.SaveChangesAsync();
    }

    private static async Task SimularVentasAsync(PasajeYaDbContext context)
    {
        var viajes = await context.Viajes
            .Include(v => v.Bus)
            .Include(v => v.Boletos)
            .Where(v => v.Salida > DateTime.Now
                && !(v.Ruta!.Origen!.Nombre == "Lima" && v.Ruta!.Destino!.Nombre == "Arequipa"))
            .ToListAsync();

        var boletos = new List<Boleto>();

        foreach (var viaje in viajes)
        {
            var libres = Enumerable.Range(1, viaje.Bus!.Capacidad)
                .Except(viaje.Boletos!
                    .Where(b => b.Estado == EstadoBoleto.Reservado || b.Estado == EstadoBoleto.Pagado)
                    .Select(b => b.NumeroAsiento))
                .Take(4)
                .ToList();

            foreach (var libre in libres)
            {
                boletos.Add(NuevoBoleto(viaje, libre, EstadoBoleto.Pagado));
            }
        }

        context.Boletos.AddRange(boletos);
        await context.SaveChangesAsync();
    }

    private static Boleto NuevoBoleto(Viaje viaje, int asiento, EstadoBoleto estado)
    {
        return new Boleto
        {
            ViajeId = viaje.Id,
            NumeroAsiento = asiento,
            Estado = estado,
            Dni = "12345678",
            Nombres = "Pasajero",
            Apellidos = "De Prueba",
            Correo = "pasajero@prueba.com"
        };
    }

}
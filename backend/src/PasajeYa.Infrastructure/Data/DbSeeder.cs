using Microsoft.EntityFrameworkCore;
using PasajeYa.Domain.Entities;

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

    }

    private static async Task SimularVentasAsync(PasajeYaDbContext context)
    {

    }

}
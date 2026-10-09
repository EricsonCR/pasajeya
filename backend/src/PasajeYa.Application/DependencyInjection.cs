using Microsoft.Extensions.DependencyInjection;
using PasajeYa.Application.Interfaces;
using PasajeYa.Application.Services;

namespace PasajeYa.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICiudadService, CiudadService>();
        services.AddScoped<IViajeService, ViajeService>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
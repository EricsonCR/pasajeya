using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasajeYa.Application.Interfaces;
using PasajeYa.Infrastructure.Data;
using PasajeYa.Infrastructure.Repositories;

namespace PasajeYa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PasajeYaDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PasajeYa")));
        services.AddScoped<ICiudadRepository, CiudadRepository>();
        services.AddScoped<IViajeRepository, ViajeRepository>();
        return services;
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasajeYa.Infrastructure.Data;

namespace PasajeYa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PasajeYaDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PasajeYa")));
        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;
using VetCare.Negocio.Interfaces;

namespace VetCare.Negocio;

public static class DependencyInjection
{
    public static IServiceCollection AddNegocio(this IServiceCollection services)
    {
        services.AddAutoMapper(configuracion => configuracion.AddProfile<MappingProfile>());
        services.AddScoped<IPropietarioService, PropietarioService>();
        services.AddScoped<IMascotaService, MascotaService>();
        return services;
    }
}

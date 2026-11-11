using AutoMapper;
using VetCare.Entidades;
using VetCare.Entidades.DTOs;

namespace VetCare.Negocio;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Propietario, PropietarioDto>()
            .ForMember(destino => destino.MascotasCount,
                opciones => opciones.MapFrom(origen => origen.Mascota.Count));

        CreateMap<PropietarioCrearDto, Propietario>();

        CreateMap<PropietarioActualizarDto, Propietario>()
            .ForMember(destino => destino.PropietarioId, opciones => opciones.Ignore())
            .ForMember(destino => destino.FechaRegistro, opciones => opciones.Ignore())
            .ForMember(destino => destino.Mascota, opciones => opciones.Ignore());

        CreateMap<Mascota, MascotaDto>()
            .ForMember(destino => destino.PropietarioNombre,
                opciones => opciones.MapFrom(origen =>
                    origen.Propietario.Nombres + " " + origen.Propietario.Apellidos));

        CreateMap<MascotaCrearDto, Mascota>()
            .ForMember(destino => destino.MascotaId, opciones => opciones.Ignore())
            .ForMember(destino => destino.FechaRegistro, opciones => opciones.Ignore())
            .ForMember(destino => destino.Propietario, opciones => opciones.Ignore());

        CreateMap<MascotaActualizarDto, Mascota>()
            .ForMember(destino => destino.MascotaId, opciones => opciones.Ignore())
            .ForMember(destino => destino.FechaRegistro, opciones => opciones.Ignore())
            .ForMember(destino => destino.Propietario, opciones => opciones.Ignore());
    }
}

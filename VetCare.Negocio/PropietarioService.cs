using AutoMapper;
using VetCare.Datos.Interfaces;
using VetCare.Entidades;
using VetCare.Entidades.DTOs;
using VetCare.Negocio.Interfaces;

namespace VetCare.Negocio;

public sealed class PropietarioService(
    IPropietarioRepository repository,
    IMapper mapper) : IPropietarioService
{
    public async Task<IReadOnlyList<PropietarioDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var propietarios = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IReadOnlyList<PropietarioDto>>(propietarios);
    }

    public async Task<PropietarioDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var propietario = await repository.GetByIdAsync(id, cancellationToken);
        return propietario is null ? null : mapper.Map<PropietarioDto>(propietario);
    }

    public async Task<PropietarioDto> CreateAsync(
        PropietarioCrearDto dto,
        CancellationToken cancellationToken = default)
    {
        if (await repository.GetByCorreoAsync(dto.Correo, cancellationToken) is not null)
        {
            throw new ReglaNegocioException("Ya existe un propietario con el correo indicado.");
        }

        var propietario = mapper.Map<Propietario>(dto);
        await repository.AddAsync(propietario, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<PropietarioDto>(propietario);
    }

    public async Task<bool> UpdateAsync(
        int id,
        PropietarioActualizarDto dto,
        CancellationToken cancellationToken = default)
    {
        var propietario = await repository.GetByIdAsync(id, cancellationToken);
        if (propietario is null)
        {
            return false;
        }

        var propietarioConCorreo = await repository.GetByCorreoAsync(dto.Correo, cancellationToken);
        if (propietarioConCorreo is not null && propietarioConCorreo.PropietarioId != id)
        {
            throw new ReglaNegocioException("Ya existe otro propietario con el correo indicado.");
        }

        mapper.Map(dto, propietario);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var propietario = await repository.GetByIdAsync(id, cancellationToken);
        if (propietario is null)
        {
            return false;
        }

        if (propietario.Mascota.Count > 0)
        {
            throw new ReglaNegocioException(
                "No se puede eliminar un propietario que tiene mascotas registradas.");
        }

        await repository.DeleteAsync(propietario, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}

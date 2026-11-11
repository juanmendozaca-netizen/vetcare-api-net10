using AutoMapper;
using VetCare.Datos.Interfaces;
using VetCare.Entidades;
using VetCare.Entidades.DTOs;
using VetCare.Negocio.Interfaces;

namespace VetCare.Negocio;

public sealed class MascotaService(
    IMascotaRepository repository,
    IPropietarioRepository propietarioRepository,
    IMapper mapper) : IMascotaService
{
    public async Task<IReadOnlyList<MascotaDto>> GetAllAsync(
        string? especie,
        string? nombre,
        CancellationToken cancellationToken = default)
    {
        var mascotas = await repository.GetAllAsync(especie, nombre, cancellationToken);
        return mapper.Map<IReadOnlyList<MascotaDto>>(mascotas);
    }

    public async Task<IReadOnlyList<MascotaDto>> GetByPropietarioIdAsync(
        int propietarioId,
        CancellationToken cancellationToken = default)
    {
        var mascotas = await repository.GetByPropietarioIdAsync(propietarioId, cancellationToken);
        return mapper.Map<IReadOnlyList<MascotaDto>>(mascotas);
    }

    public async Task<MascotaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var mascota = await repository.GetByIdAsync(id, cancellationToken);
        return mascota is null ? null : mapper.Map<MascotaDto>(mascota);
    }

    public async Task<MascotaDto> CreateAsync(
        MascotaCrearDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(dto.PropietarioId, dto.NumeroMicrochip, null, cancellationToken);

        var mascota = mapper.Map<Mascota>(dto);
        await repository.AddAsync(mascota, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var mascotaCreada = await repository.GetByIdAsync(mascota.MascotaId, cancellationToken);
        return mapper.Map<MascotaDto>(mascotaCreada);
    }

    public async Task<bool> UpdateAsync(
        int id,
        MascotaActualizarDto dto,
        CancellationToken cancellationToken = default)
    {
        var mascota = await repository.GetByIdAsync(id, cancellationToken);
        if (mascota is null)
        {
            return false;
        }

        await ValidateAsync(dto.PropietarioId, dto.NumeroMicrochip, id, cancellationToken);
        mapper.Map(dto, mascota);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var mascota = await repository.GetByIdAsync(id, cancellationToken);
        if (mascota is null)
        {
            return false;
        }

        await repository.DeleteAsync(mascota, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task ValidateAsync(
        int propietarioId,
        string numeroMicrochip,
        int? mascotaId,
        CancellationToken cancellationToken)
    {
        if (await propietarioRepository.GetByIdAsync(propietarioId, cancellationToken) is null)
        {
            throw new ReglaNegocioException("El propietario indicado no existe.");
        }

        var mascotaConMicrochip = await repository.GetByMicrochipAsync(numeroMicrochip, cancellationToken);
        if (mascotaConMicrochip is not null && mascotaConMicrochip.MascotaId != mascotaId)
        {
            throw new ReglaNegocioException("Ya existe una mascota con el número de microchip indicado.");
        }
    }
}

using VetCare.Entidades.DTOs;

namespace VetCare.Negocio.Interfaces;

public interface IMascotaService
{
    Task<IReadOnlyList<MascotaDto>> GetAllAsync(string? especie, string? nombre, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MascotaDto>> GetByPropietarioIdAsync(int propietarioId, CancellationToken cancellationToken = default);
    Task<MascotaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MascotaDto> CreateAsync(MascotaCrearDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, MascotaActualizarDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

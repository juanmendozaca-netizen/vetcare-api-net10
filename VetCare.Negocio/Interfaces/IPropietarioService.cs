using VetCare.Entidades.DTOs;

namespace VetCare.Negocio.Interfaces;

public interface IPropietarioService
{
    Task<IReadOnlyList<PropietarioDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PropietarioDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PropietarioDto> CreateAsync(PropietarioCrearDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, PropietarioActualizarDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

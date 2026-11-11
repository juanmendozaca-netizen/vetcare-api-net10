using VetCare.Entidades;

namespace VetCare.Datos.Interfaces;

public interface IMascotaRepository
{
    Task<IReadOnlyList<Mascota>> GetAllAsync(string? especie, string? nombre, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Mascota>> GetByPropietarioIdAsync(int propietarioId, CancellationToken cancellationToken = default);
    Task<Mascota?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Mascota?> GetByMicrochipAsync(string microchip, CancellationToken cancellationToken = default);
    Task AddAsync(Mascota mascota, CancellationToken cancellationToken = default);
    Task DeleteAsync(Mascota mascota, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

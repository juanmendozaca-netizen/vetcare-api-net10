using VetCare.Entidades;

namespace VetCare.Datos.Interfaces;

public interface IPropietarioRepository
{
    Task<IReadOnlyList<Propietario>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Propietario?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Propietario?> GetByCorreoAsync(string correo, CancellationToken cancellationToken = default);
    Task AddAsync(Propietario propietario, CancellationToken cancellationToken = default);
    Task DeleteAsync(Propietario propietario, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

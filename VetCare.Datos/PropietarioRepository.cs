using Microsoft.EntityFrameworkCore;
using VetCare.Datos.Interfaces;
using VetCare.Entidades;

namespace VetCare.Datos;

public sealed class PropietarioRepository(VetCareContext context) : IPropietarioRepository
{
    public async Task<IReadOnlyList<Propietario>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Propietarios
            .Include(propietario => propietario.Mascota)
            .AsNoTracking()
            .OrderBy(propietario => propietario.Apellidos)
            .ThenBy(propietario => propietario.Nombres)
            .ToListAsync(cancellationToken);

    public Task<Propietario?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Propietarios
            .Include(propietario => propietario.Mascota)
            .FirstOrDefaultAsync(propietario => propietario.PropietarioId == id, cancellationToken);

    public Task<Propietario?> GetByCorreoAsync(string correo, CancellationToken cancellationToken = default) =>
        context.Propietarios
            .FirstOrDefaultAsync(propietario => propietario.Correo == correo, cancellationToken);

    public async Task AddAsync(Propietario propietario, CancellationToken cancellationToken = default) =>
        await context.Propietarios.AddAsync(propietario, cancellationToken);

    public Task DeleteAsync(Propietario propietario, CancellationToken cancellationToken = default)
    {
        context.Propietarios.Remove(propietario);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}

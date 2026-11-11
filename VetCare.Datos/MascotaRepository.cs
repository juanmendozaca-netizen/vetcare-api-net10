using Microsoft.EntityFrameworkCore;
using VetCare.Datos.Interfaces;
using VetCare.Entidades;

namespace VetCare.Datos;

public sealed class MascotaRepository(VetCareContext context) : IMascotaRepository
{
    public async Task<IReadOnlyList<Mascota>> GetAllAsync(
        string? especie,
        string? nombre,
        CancellationToken cancellationToken = default)
    {
        var query = context.Mascotas
            .Include(mascota => mascota.Propietario)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(especie))
        {
            query = query.Where(mascota => mascota.Especie == especie);
        }

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(mascota => mascota.Nombre.Contains(nombre));
        }

        return await query
            .OrderBy(mascota => mascota.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Mascota>> GetByPropietarioIdAsync(
        int propietarioId,
        CancellationToken cancellationToken = default) =>
        await context.Mascotas
            .Include(mascota => mascota.Propietario)
            .Where(mascota => mascota.PropietarioId == propietarioId)
            .AsNoTracking()
            .OrderBy(mascota => mascota.Nombre)
            .ToListAsync(cancellationToken);

    public Task<Mascota?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Mascotas
            .Include(mascota => mascota.Propietario)
            .FirstOrDefaultAsync(mascota => mascota.MascotaId == id, cancellationToken);

    public Task<Mascota?> GetByMicrochipAsync(string microchip, CancellationToken cancellationToken = default) =>
        context.Mascotas
            .FirstOrDefaultAsync(mascota => mascota.NumeroMicrochip == microchip, cancellationToken);

    public async Task AddAsync(Mascota mascota, CancellationToken cancellationToken = default) =>
        await context.Mascotas.AddAsync(mascota, cancellationToken);

    public Task DeleteAsync(Mascota mascota, CancellationToken cancellationToken = default)
    {
        context.Mascotas.Remove(mascota);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}

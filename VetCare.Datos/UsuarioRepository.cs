using Microsoft.EntityFrameworkCore;
using VetCare.Datos.Interfaces;
using VetCare.Entidades;

namespace VetCare.Datos;

public sealed class UsuarioRepository(VetCareContext context) : IUsuarioRepository
{
    public Task<Usuario?> GetByCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default) =>
        context.Usuarios.FirstOrDefaultAsync(
            usuario => usuario.Correo == correo,
            cancellationToken);

    public async Task AddAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default) =>
        await context.Usuarios.AddAsync(usuario, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}

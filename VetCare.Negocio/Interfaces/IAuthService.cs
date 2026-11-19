using VetCare.Entidades.DTOs;

namespace VetCare.Negocio.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegistroDto dto, CancellationToken cancellationToken = default);

    Task<TokenDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
}

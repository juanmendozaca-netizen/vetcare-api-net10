using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VetCare.Datos.Interfaces;
using VetCare.Entidades;
using VetCare.Entidades.DTOs;
using VetCare.Negocio.Interfaces;

namespace VetCare.Negocio;

public sealed class AuthService(
    IUsuarioRepository repository,
    IConfiguration configuration) : IAuthService
{
    public async Task RegisterAsync(
        RegistroDto dto,
        CancellationToken cancellationToken = default)
    {
        if (await repository.GetByCorreoAsync(dto.Correo, cancellationToken) is not null)
        {
            throw new ReglaNegocioException("Ya existe un usuario con el correo indicado.");
        }

        var usuario = new Usuario
        {
            Correo = dto.Correo,
            ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Clave),
            Rol = "Recepcionista"
        };

        await repository.AddAsync(usuario, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<TokenDto?> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        var usuario = await repository.GetByCorreoAsync(dto.Correo, cancellationToken);
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.ClaveHash))
        {
            return null;
        }

        return CreateToken(usuario);
    }

    private TokenDto CreateToken(Usuario usuario)
    {
        var key = configuration["Jwt:Key"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");

        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
        {
            throw new InvalidOperationException("La clave JWT debe tener al menos 32 caracteres.");
        }

        var expiracion = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim("rol", usuario.Rol)
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiracion,
            signingCredentials: credentials);

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiracion = expiracion
        };
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.Entidades.DTOs;
using VetCare.Negocio;
using VetCare.Negocio.Interfaces;

namespace VetCare.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService service) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("registro")]
    public async Task<IActionResult> Registro(
        RegistroDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await service.RegisterAsync(dto, cancellationToken);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (ReglaNegocioException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(
        LoginDto dto,
        CancellationToken cancellationToken)
    {
        var token = await service.LoginAsync(dto, cancellationToken);
        return token is null ? Unauthorized() : Ok(token);
    }

    [Authorize]
    [HttpGet("perfil")]
    public ActionResult<PerfilDto> Perfil()
    {
        var correo = User.FindFirstValue("email");
        var rol = User.FindFirstValue("rol");

        return Ok(new PerfilDto
        {
            Correo = correo ?? string.Empty,
            Rol = rol ?? string.Empty
        });
    }
}

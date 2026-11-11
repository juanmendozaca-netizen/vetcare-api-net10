using Microsoft.AspNetCore.Mvc;
using VetCare.Entidades.DTOs;
using VetCare.Negocio;
using VetCare.Negocio.Interfaces;

namespace VetCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PropietariosController(IPropietarioService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PropietarioDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PropietarioDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var propietario = await service.GetByIdAsync(id, cancellationToken);
        return propietario is null ? NotFound() : Ok(propietario);
    }

    [HttpPost]
    public async Task<ActionResult<PropietarioDto>> Create(
        PropietarioCrearDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var propietario = await service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = propietario.PropietarioId }, propietario);
        }
        catch (ReglaNegocioException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        PropietarioActualizarDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            return await service.UpdateAsync(id, dto, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (ReglaNegocioException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            return await service.DeleteAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (ReglaNegocioException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}

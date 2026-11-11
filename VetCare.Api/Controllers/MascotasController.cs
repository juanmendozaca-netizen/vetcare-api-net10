using Microsoft.AspNetCore.Mvc;
using VetCare.Entidades.DTOs;
using VetCare.Negocio;
using VetCare.Negocio.Interfaces;

namespace VetCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MascotasController(IMascotaService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MascotaDto>>> GetAll(
        [FromQuery] string? especie,
        [FromQuery] string? nombre,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(especie, nombre, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MascotaDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var mascota = await service.GetByIdAsync(id, cancellationToken);
        return mascota is null ? NotFound() : Ok(mascota);
    }

    [HttpGet("/api/propietarios/{propietarioId:int}/mascotas")]
    public async Task<ActionResult<IReadOnlyList<MascotaDto>>> GetByPropietario(
        int propietarioId,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetByPropietarioIdAsync(propietarioId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<MascotaDto>> Create(
        MascotaCrearDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var mascota = await service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = mascota.MascotaId }, mascota);
        }
        catch (ReglaNegocioException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        MascotaActualizarDto dto,
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

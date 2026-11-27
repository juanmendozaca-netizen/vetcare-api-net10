using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.VeterinariosApi.Models;
using VetCare.VeterinariosApi.Services;

namespace VetCare.VeterinariosApi.Controllers;

[ApiController]
[Authorize]
[Route("api/veterinarios")]
public sealed class VeterinariosController(IVeterinarioService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<VeterinarioDto>> GetAll([FromQuery] string? especialidad)
    {
        return Ok(service.GetAll(especialidad));
    }

    [HttpGet("{id:int}")]
    public ActionResult<VeterinarioDto> GetById(int id)
    {
        var veterinario = service.GetById(id);
        return veterinario is null ? NotFound() : Ok(veterinario);
    }
}


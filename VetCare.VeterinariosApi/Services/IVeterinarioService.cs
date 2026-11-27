using VetCare.VeterinariosApi.Models;

namespace VetCare.VeterinariosApi.Services;

public interface IVeterinarioService
{
    IReadOnlyList<VeterinarioDto> GetAll(string? especialidad = null);

    VeterinarioDto? GetById(int id);
}


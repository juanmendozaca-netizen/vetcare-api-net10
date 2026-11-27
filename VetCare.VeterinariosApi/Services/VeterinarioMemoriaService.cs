using VetCare.VeterinariosApi.Models;

namespace VetCare.VeterinariosApi.Services;

public sealed class VeterinarioMemoriaService : IVeterinarioService
{
    private static readonly IReadOnlyList<VeterinarioDto> Veterinarios =
    [
        new VeterinarioDto
        {
            VeterinarioId = 1,
            Nombres = "Carlos",
            Apellidos = "Salazar Rojas",
            Especialidad = "Cirugía General",
            NumeroColegiatura = "CMVP-10452",
            Turno = "Mañana",
            Correo = "carlos.salazar@vetcare.com"
        },
        new VeterinarioDto
        {
            VeterinarioId = 2,
            Nombres = "Valeria",
            Apellidos = "Paredes Huamán",
            Especialidad = "Dermatología Veterinaria",
            NumeroColegiatura = "CMVP-11289",
            Turno = "Tarde",
            Correo = "valeria.paredes@vetcare.com"
        },
        new VeterinarioDto
        {
            VeterinarioId = 3,
            Nombres = "Miguel",
            Apellidos = "Quispe Alarcón",
            Especialidad = "Medicina Interna y Diagnóstico",
            NumeroColegiatura = "CMVP-09874",
            Turno = "Mañana",
            Correo = "miguel.quispe@vetcare.com"
        },
        new VeterinarioDto
        {
            VeterinarioId = 4,
            Nombres = "Lucía",
            Apellidos = "Fernández Castro",
            Especialidad = "Animales Exóticos",
            NumeroColegiatura = "CMVP-12031",
            Turno = "Noche",
            Correo = "lucia.fernandez@vetcare.com"
        }
    ];

    public IReadOnlyList<VeterinarioDto> GetAll(string? especialidad = null)
    {
        if (string.IsNullOrWhiteSpace(especialidad))
        {
            return Veterinarios;
        }

        return Veterinarios
            .Where(v => v.Especialidad.Contains(especialidad, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public VeterinarioDto? GetById(int id) =>
        Veterinarios.FirstOrDefault(v => v.VeterinarioId == id);
}


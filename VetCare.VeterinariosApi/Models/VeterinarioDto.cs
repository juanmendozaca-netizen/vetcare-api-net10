namespace VetCare.VeterinariosApi.Models;

public sealed class VeterinarioDto
{
    public int VeterinarioId { get; init; }

    public string Nombres { get; init; } = string.Empty;

    public string Apellidos { get; init; } = string.Empty;

    public string Especialidad { get; init; } = string.Empty;

    public string NumeroColegiatura { get; init; } = string.Empty;

    public string Turno { get; init; } = string.Empty;

    public string Correo { get; init; } = string.Empty;
}


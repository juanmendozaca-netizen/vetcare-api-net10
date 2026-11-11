namespace VetCare.Entidades.DTOs;

public sealed class PropietarioDto
{
    public int PropietarioId { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public int MascotasCount { get; set; }
}

namespace VetCare.Entidades.DTOs;

public sealed class MascotaDto
{
    public int MascotaId { get; set; }
    public int PropietarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string? Raza { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public decimal? Peso { get; set; }
    public string NumeroMicrochip { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public string PropietarioNombre { get; set; } = string.Empty;
}

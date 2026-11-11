using System.ComponentModel.DataAnnotations;

namespace VetCare.Entidades.DTOs;

public sealed class MascotaCrearDto
{
    [Required]
    public int PropietarioId { get; set; }

    [Required, StringLength(80, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Especie { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Raza { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    [Range(0.01, 9999.99)]
    public decimal? Peso { get; set; }

    [Required, StringLength(30, MinimumLength = 3)]
    public string NumeroMicrochip { get; set; } = string.Empty;
}

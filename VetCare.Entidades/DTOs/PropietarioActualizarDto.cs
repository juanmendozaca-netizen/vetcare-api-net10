using System.ComponentModel.DataAnnotations;

namespace VetCare.Entidades.DTOs;

public sealed class PropietarioActualizarDto
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string Nombres { get; set; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 2)]
    public string Apellidos { get; set; } = string.Empty;

    [Required, StringLength(20, MinimumLength = 7)]
    public string Telefono { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Correo { get; set; } = string.Empty;
}

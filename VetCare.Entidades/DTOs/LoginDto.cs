namespace VetCare.Entidades.DTOs;

public sealed class LoginDto
{
    public string Correo { get; set; } = null!;

    public string Clave { get; set; } = null!;
}

namespace VetCare.Entidades.DTOs;

public sealed class TokenDto
{
    public string Token { get; set; } = null!;

    public DateTime Expiracion { get; set; }
}

using System;
using System.Collections.Generic;

namespace VetCare.Entidades;

public partial class Propietario
{
    public int PropietarioId { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<Mascota> Mascota { get; set; } = new List<Mascota>();
}

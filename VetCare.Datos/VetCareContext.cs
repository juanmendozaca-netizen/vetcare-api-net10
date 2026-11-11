using Microsoft.EntityFrameworkCore;
using VetCare.Entidades;

namespace VetCare.Datos;

public partial class VetCareContext : DbContext
{
    public VetCareContext(DbContextOptions<VetCareContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mascota> Mascotas { get; set; }

    public virtual DbSet<Propietario> Propietarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasIndex(e => e.NumeroMicrochip, "UQ_Mascotas_NumeroMicrochip").IsUnique();

            entity.Property(e => e.Especie).HasMaxLength(40);
            entity.Property(e => e.FechaRegistro)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Mascotas_FechaRegistro");
            entity.Property(e => e.Nombre).HasMaxLength(80);
            entity.Property(e => e.NumeroMicrochip).HasMaxLength(30);
            entity.Property(e => e.Peso).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Raza).HasMaxLength(80);

            entity.HasOne(d => d.Propietario).WithMany(p => p.Mascota)
                .HasForeignKey(d => d.PropietarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mascotas_Propietarios");
        });

        modelBuilder.Entity<Propietario>(entity =>
        {
            entity.HasIndex(e => e.Correo, "UQ_Propietarios_Correo").IsUnique();

            entity.Property(e => e.Apellidos).HasMaxLength(80);
            entity.Property(e => e.Correo).HasMaxLength(150);
            entity.Property(e => e.FechaRegistro)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Propietarios_FechaRegistro");
            entity.Property(e => e.Nombres).HasMaxLength(80);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

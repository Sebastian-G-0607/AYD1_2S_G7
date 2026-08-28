using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class EstudianteEntityConfiguration : IEntityTypeConfiguration<Estudiante>
{
    public void Configure(EntityTypeBuilder<Estudiante> builder)
    {
        builder.ToTable("estudiantes");

        builder.HasKey(e => e.UsuarioId);

        builder.Property(e => e.UsuarioId)
            .HasColumnName("usuario_id")
            .ValueGeneratedNever();

        builder.Property(e => e.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Apellido)
            .HasColumnName("apellido")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Carnet)
            .HasColumnName("carnet")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => e.Carnet)
            .IsUnique();

        builder.Property(e => e.Genero)
            .HasColumnName("genero")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Direccion)
            .HasColumnName("direccion")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(20)
            .IsRequired();

        // builder.Property(e => e.FechaNacimiento)
        //     .HasColumnName("fecha_nacimiento")
        //     .HasColumnType("date")
        //     .IsRequired();
        builder.Property(e => e.FechaNacimiento)
            .HasColumnName("fecha_nacimiento")
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(e => e.FotografiaUrl)
            .HasColumnName("fotografia_url")
            .HasMaxLength(255);

        builder.HasOne(e => e.Usuario)
            .WithOne(u => u.Estudiante)
            .HasForeignKey<Estudiante>(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

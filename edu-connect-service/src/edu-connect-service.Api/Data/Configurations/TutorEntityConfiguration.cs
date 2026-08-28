using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class TutorEntityConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        // builder.ToTable("tutores", t =>
        // {
        //     t.HasCheckConstraint("check_rango_horario", "hora_fin > hora_inicio");
        // });
        builder.ToTable("tutores", t =>
        {
            t.HasCheckConstraint("check_rango_horario", "\"hora_fin\" > \"hora_inicio\"");
        });

        builder.HasKey(t => t.UsuarioId);

        builder.Property(t => t.UsuarioId)
            .HasColumnName("usuario_id")
            .ValueGeneratedNever();

        builder.Property(t => t.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Apellido)
            .HasColumnName("apellido")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.CarnetId)
            .HasColumnName("carnet_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.CarnetId)
            .IsUnique();

        builder.Property(t => t.NumeroIdentificacion)
            .HasColumnName("numero_identificacion")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.NumeroIdentificacion)
            .IsUnique();

        builder.Property(t => t.Genero)
            .HasColumnName("genero")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Direccion)
            .HasColumnName("direccion")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(20)
            .IsRequired();

        // builder.Property(t => t.FechaNacimiento)
        //     .HasColumnName("fecha_nacimiento")
        //     .HasColumnType("date")
        //     .IsRequired();
        builder.Property(t => t.FechaNacimiento)
            .HasColumnName("fecha_nacimiento")
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(t => t.FotografiaUrl)
            .HasColumnName("fotografia_url")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.DireccionTutoria)
            .HasColumnName("direccion_tutoria")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.AnioInicio)
            .HasColumnName("anio_inicio")
            .IsRequired();

        builder.Property(t => t.Universidad)
            .HasColumnName("universidad")
            .HasMaxLength(150)
            .IsRequired();

        // builder.Property(t => t.HoraInicio)
        //     .HasColumnName("hora_inicio")
        //     .HasColumnType("time without time zone");
        builder.Property(t => t.HoraInicio)
            .HasColumnName("hora_inicio")
            .HasColumnType("INTERVAL DAY(0) TO SECOND(0)");

        // builder.Property(t => t.HoraFin)
        //     .HasColumnName("hora_fin")
        //     .HasColumnType("time without time zone");
        builder.Property(t => t.HoraFin)
            .HasColumnName("hora_fin")
            .HasColumnType("INTERVAL DAY(0) TO SECOND(0)");

        builder.HasOne(t => t.Usuario)
            .WithOne(u => u.Tutor)
            .HasForeignKey<Tutor>(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

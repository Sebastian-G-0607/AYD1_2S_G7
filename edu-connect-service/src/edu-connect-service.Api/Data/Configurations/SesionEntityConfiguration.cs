using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class SesionEntityConfiguration : IEntityTypeConfiguration<Sesion>
{
    public void Configure(EntityTypeBuilder<Sesion> builder)
    {
        builder.ToTable("sesiones");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.EstudianteId)
            .HasColumnName("estudiante_id")
            .IsRequired();

        builder.Property(s => s.TutorId)
            .HasColumnName("tutor_id")
            .IsRequired();

        builder.Property(s => s.MateriaId)
            .HasColumnName("materia_id")
            .IsRequired();

        builder.Property(s => s.EstadoId)
            .HasColumnName("estado_id")
            .IsRequired();

        // builder.Property(s => s.FechaSesion)
        //     .HasColumnName("fecha_sesion")
        //     .HasColumnType("date")
        //     .IsRequired();
        builder.Property(s => s.FechaSesion)
            .HasColumnName("fecha_sesion")
            .HasColumnType("DATE")
            .IsRequired();

        // builder.Property(s => s.HoraInicio)
        //     .HasColumnName("hora_inicio")
        //     .HasColumnType("time without time zone")
        //     .IsRequired();
        builder.Property(s => s.HoraInicio)
            .HasColumnName("hora_inicio")
            .HasColumnType("INTERVAL DAY(0) TO SECOND(0)")
            .IsRequired();

        // builder.Property(s => s.HoraFin)
        //     .HasColumnName("hora_fin")
        //     .HasColumnType("time without time zone");
        builder.Property(s => s.HoraFin)
            .HasColumnName("hora_fin")
            .HasColumnType("INTERVAL DAY(0) TO SECOND(0)");

        // builder.Property(s => s.Motivo)
        //     .HasColumnName("motivo")
        //     .HasColumnType("text")
        //     .IsRequired();
        builder.Property(s => s.Motivo)
            .HasColumnName("motivo")
            .HasColumnType("CLOB")
            .IsRequired();

        // builder.Property(s => s.Resumen)
        //     .HasColumnName("resumen")
        //     .HasColumnType("text");
        builder.Property(s => s.Resumen)
            .HasColumnName("resumen")
            .HasColumnType("CLOB");

        builder.Property(s => s.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(s => s.Estudiante)
            .WithMany(e => e.Sesiones)
            .HasForeignKey(s => s.EstudianteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Tutor)
            .WithMany(t => t.Sesiones)
            .HasForeignKey(s => s.TutorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Materia)
            .WithMany(m => m.Sesiones)
            .HasForeignKey(s => s.MateriaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Estado)
            .WithMany(es => es.Sesiones)
            .HasForeignKey(s => s.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

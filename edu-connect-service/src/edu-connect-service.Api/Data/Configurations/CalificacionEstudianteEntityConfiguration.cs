using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class CalificacionEstudianteEntityConfiguration : IEntityTypeConfiguration<CalificacionEstudiante>
{
    public void Configure(EntityTypeBuilder<CalificacionEstudiante> builder)
    {
        builder.ToTable("calificaciones_estudiantes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.SesionId)
            .HasColumnName("sesion_id")
            .IsRequired();

        builder.HasIndex(c => c.SesionId)
            .IsUnique();

        builder.Property(c => c.Estrellas)
            .HasColumnName("estrellas")
            .IsRequired();

        builder.Property(c => c.Comentario)
            .HasColumnName("comentario")
            .HasColumnType("CLOB");

        builder.Property(c => c.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(c => c.Sesion)
            .WithOne(s => s.CalificacionEstudiante)
            .HasForeignKey<CalificacionEstudiante>(c => c.SesionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

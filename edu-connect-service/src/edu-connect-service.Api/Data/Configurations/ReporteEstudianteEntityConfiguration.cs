using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class ReporteEstudianteEntityConfiguration : IEntityTypeConfiguration<ReporteEstudiante>
{
    public void Configure(EntityTypeBuilder<ReporteEstudiante> builder)
    {
        builder.ToTable("reportes_estudiantes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.SesionId)
            .HasColumnName("sesion_id")
            .IsRequired();

        builder.Property(r => r.CategoriaId)
            .HasColumnName("categoria_id")
            .IsRequired();

        builder.Property(r => r.Motivo)
            .HasColumnName("motivo")
            .HasColumnType("CLOB")
            .IsRequired();

        builder.Property(r => r.FechaReporte)
            .HasColumnName("fecha_reporte")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.Estado)
            .HasColumnName("estado")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(r => r.Sesion)
            .WithMany(s => s.ReportesEstudiante)
            .HasForeignKey(r => r.SesionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Categoria)
            .WithMany(c => c.Reportes)
            .HasForeignKey(r => r.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

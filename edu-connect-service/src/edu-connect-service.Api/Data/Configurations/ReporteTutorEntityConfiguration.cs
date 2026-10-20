using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class ReporteTutorEntityConfiguration : IEntityTypeConfiguration<ReporteTutor>
{
    public void Configure(EntityTypeBuilder<ReporteTutor> builder)
    {
        builder.ToTable("reportes_tutores");

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
            .WithMany(s => s.ReportesTutor)
            .HasForeignKey(r => r.SesionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Categoria)
            .WithMany(c => c.Reportes)
            .HasForeignKey(r => r.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

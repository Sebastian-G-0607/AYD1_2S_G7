using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class PlanEstudioEntityConfiguration : IEntityTypeConfiguration<PlanEstudio>
{
    public void Configure(EntityTypeBuilder<PlanEstudio> builder)
    {
        builder.ToTable("planes_estudio");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.SesionId)
            .HasColumnName("sesion_id")
            .IsRequired();

        builder.HasIndex(p => p.SesionId)
            .IsUnique();

        builder.Property(p => p.DificultadesIdentificadas)
            .HasColumnName("dificultades_identificadas")
            .HasColumnType("CLOB")
            .IsRequired();

        builder.Property(p => p.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(p => p.Sesion)
            .WithOne(s => s.PlanEstudio)
            .HasForeignKey<PlanEstudio>(p => p.SesionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

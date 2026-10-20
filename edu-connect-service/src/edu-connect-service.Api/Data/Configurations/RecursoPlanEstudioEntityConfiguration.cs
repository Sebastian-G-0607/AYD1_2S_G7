using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class RecursoPlanEstudioEntityConfiguration : IEntityTypeConfiguration<RecursoPlanEstudio>
{
    public void Configure(EntityTypeBuilder<RecursoPlanEstudio> builder)
    {
        builder.ToTable("recursos_plan_estudio");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.PlanEstudioId)
            .HasColumnName("plan_estudio_id")
            .IsRequired();

        builder.Property(r => r.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.Tipo)
            .HasColumnName("tipo")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.DescripcionUso)
            .HasColumnName("descripcion_uso")
            .HasColumnType("CLOB");

        builder.HasOne(r => r.PlanEstudio)
            .WithMany(p => p.Recursos)
            .HasForeignKey(r => r.PlanEstudioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

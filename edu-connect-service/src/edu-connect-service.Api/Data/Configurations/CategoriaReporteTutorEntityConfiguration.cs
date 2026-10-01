using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class CategoriaReporteTutorEntityConfiguration : IEntityTypeConfiguration<CategoriaReporteTutor>
{
    public void Configure(EntityTypeBuilder<CategoriaReporteTutor> builder)
    {
        builder.ToTable("categorias_reportes_tutores");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(c => c.Nombre)
            .IsUnique();

        builder.Property(c => c.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(255);
    }
}

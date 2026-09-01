using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class EstadoSesionEntityConfiguration : IEntityTypeConfiguration<EstadoSesion>
{
    public void Configure(EntityTypeBuilder<EstadoSesion> builder)
    {
        builder.ToTable("estados_sesiones");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => e.Nombre)
            .IsUnique();

        builder.Property(e => e.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(255);
    }
}

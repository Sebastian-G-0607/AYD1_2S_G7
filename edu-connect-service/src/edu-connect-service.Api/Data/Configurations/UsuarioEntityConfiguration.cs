using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class UsuarioEntityConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Correo)
            .HasColumnName("correo")
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(u => u.Correo)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.RolId)
            .HasColumnName("rol_id")
            .IsRequired();

        builder.Property(u => u.EstadoId)
            .HasColumnName("estado_id")
            .IsRequired();

        builder.Property(u => u.FechaRegistro)
            .HasColumnName("fecha_registro")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(u => u.FechaBaja)
            .HasColumnName("fecha_baja");

        // builder.Property(u => u.MotivoBaja)
        //     .HasColumnName("motivo_baja")
        //     .HasColumnType("text");
        builder.Property(u => u.MotivoBaja)
            .HasColumnName("motivo_baja")
            .HasColumnType("CLOB");

        builder.HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId);

        builder.HasOne(u => u.Estado)
            .WithMany(e => e.Usuarios)
            .HasForeignKey(u => u.EstadoId);
    }
}

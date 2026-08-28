using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class AdministradorEntityConfiguration : IEntityTypeConfiguration<Administrador>
{
    public void Configure(EntityTypeBuilder<Administrador> builder)
    {
        builder.ToTable("administradores");

        builder.HasKey(a => a.UsuarioId);

        builder.Property(a => a.UsuarioId)
            .HasColumnName("usuario_id")
            .ValueGeneratedNever();

        builder.Property(a => a.PasswordFase2Hash)
            .HasColumnName("password_fase2_hash")
            .HasMaxLength(255)
            .IsRequired();

        builder.HasOne(a => a.Usuario)
            .WithOne(u => u.Administrador)
            .HasForeignKey<Administrador>(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

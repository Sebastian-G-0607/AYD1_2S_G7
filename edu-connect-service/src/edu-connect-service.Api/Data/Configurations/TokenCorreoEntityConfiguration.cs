using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class TokenCorreoEntityConfiguration : IEntityTypeConfiguration<TokenCorreo>
{
    public void Configure(EntityTypeBuilder<TokenCorreo> builder)
    {
        builder.ToTable("token_correo");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.UsuarioId)
            .HasColumnName("usuario_id")
            .IsRequired();

        builder.Property(t => t.Token)
            .HasColumnName("token")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.FechaGeneracion)
            .HasColumnName("fecha_generacion")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(t => t.FechaExpiracion)
            .HasColumnName("fecha_expiracion")
            .IsRequired();

        builder.Property(t => t.Revocado)
            .HasColumnName("revocado")
            .HasColumnType("NUMBER(1)")
            .HasDefaultValue(false);

        builder.HasOne(t => t.Usuario)
            .WithMany(u => u.TokensCorreo)
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class TutorDiaAtencionEntityConfiguration : IEntityTypeConfiguration<TutorDiaAtencion>
{
    public void Configure(EntityTypeBuilder<TutorDiaAtencion> builder)
    {
        // builder.ToTable("tutores_dias_atencion", t =>
        // {
        //     t.HasCheckConstraint("check_dia_semana", "dia_semana BETWEEN 1 AND 7");
        // });
        builder.ToTable("tutores_dias_atencion", t =>
        {
            t.HasCheckConstraint("check_dia_semana", "\"dia_semana\" BETWEEN 1 AND 7");
        });

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.TutorId)
            .HasColumnName("tutor_id")
            .IsRequired();

        builder.Property(d => d.DiaSemana)
            .HasColumnName("dia_semana")
            .IsRequired();

        builder.HasIndex(d => new { d.TutorId, d.DiaSemana })
            .IsUnique();

        builder.HasOne(d => d.Tutor)
            .WithMany(t => t.DiasAtencion)
            .HasForeignKey(d => d.TutorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

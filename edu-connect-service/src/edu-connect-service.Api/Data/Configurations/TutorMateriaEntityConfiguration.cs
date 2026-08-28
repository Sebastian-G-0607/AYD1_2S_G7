using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace edu_connect_service.Api.Data.Configurations;

public class TutorMateriaEntityConfiguration : IEntityTypeConfiguration<TutorMateria>
{
    public void Configure(EntityTypeBuilder<TutorMateria> builder)
    {
        builder.ToTable("tutores_materias");

        builder.HasKey(tm => new { tm.TutorId, tm.MateriaId });

        builder.Property(tm => tm.TutorId)
            .HasColumnName("tutor_id");

        builder.Property(tm => tm.MateriaId)
            .HasColumnName("materia_id");

        builder.HasOne(tm => tm.Tutor)
            .WithMany(t => t.TutorMaterias)
            .HasForeignKey(tm => tm.TutorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tm => tm.Materia)
            .WithMany(m => m.TutorMaterias)
            .HasForeignKey(tm => tm.MateriaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

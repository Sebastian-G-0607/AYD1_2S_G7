using edu_connect_service.Api.Data.Configurations;
using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Data;

public class edu_connect_serviceContext(DbContextOptions<edu_connect_serviceContext> options)
    : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Rol> Roles => Set<Rol>();

    public DbSet<EstadoUsuario> EstadosUsuarios => Set<EstadoUsuario>();

    public DbSet<EstadoSesion> EstadosSesiones => Set<EstadoSesion>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Administrador> Administradores => Set<Administrador>();

    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();

    public DbSet<Tutor> Tutores => Set<Tutor>();

    public DbSet<TutorDiaAtencion> TutoresDiasAtencion => Set<TutorDiaAtencion>();

    public DbSet<Materia> Materias => Set<Materia>();

    public DbSet<TutorMateria> TutoresMaterias => Set<TutorMateria>();

    public DbSet<Sesion> Sesiones => Set<Sesion>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>();

        configurationBuilder.Properties<TimeOnly>()
            .HaveConversion<TimeOnlyConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItemEntityConfiguration).Assembly);
    }
}

public class DateOnlyConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(
            d => d.ToDateTime(TimeOnly.MinValue),
            d => DateOnly.FromDateTime(d))
    {
    }
}

public class TimeOnlyConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TimeOnly, TimeSpan>
{
    public TimeOnlyConverter()
        : base(
            t => t.ToTimeSpan(),
            t => TimeOnly.FromTimeSpan(t))
    {
    }
}

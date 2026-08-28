using System.Linq.Expressions;
using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Data;

public static class DataExtensions
{
    // public static WebApplicationBuilder Addedu_connect_serviceNpgsql<TContext>(
    //     this WebApplicationBuilder builder,
    //     string connectionStringName) where TContext : DbContext
    // {
    //     var connectionString = builder.Configuration.GetConnectionString(connectionStringName);
    //     builder.Services.AddDbContext<TContext>(options =>
    //     {
    //         options.UseNpgsql(connectionString);
    //         ConfigureDbContext(options);
    //     });
    //     return builder;
    // }

    public static WebApplicationBuilder Addedu_connect_serviceOracle<TContext>(
        this WebApplicationBuilder builder,
        string connectionStringName) where TContext : DbContext
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionStringName);

        builder.Services.AddDbContext<TContext>(options =>
        {
            options.UseOracle(connectionString, o => o.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19));
            ConfigureDbContext(options);
        });

        return builder;
    }

    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            edu_connect_serviceContext dbContext = scope.ServiceProvider.
                        GetRequiredService<edu_connect_serviceContext>();            
            await dbContext.Database.MigrateAsync();
        }
        catch(Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }

    private static DbContextOptionsBuilder ConfigureDbContext(DbContextOptionsBuilder options)
    {
        return options.UseSeeding((context, _) =>
                    {
                        SeedCatalogsAndData(context);
                    })
                    .UseAsyncSeeding(async (context, _, cancellationToken) =>
                    {
                        await SeedCatalogsAndDataAsync(context, cancellationToken);
                    });
    }

    private static void SeedCatalogsAndData(DbContext context)
    {
        var hasChanges = false;

        if (!context.Set<Rol>().Any())
        {
            SeedRoles(context);
            hasChanges = true;
        }

        if (!context.Set<EstadoUsuario>().Any())
        {
            SeedEstadosUsuarios(context);
            hasChanges = true;
        }

        if (!context.Set<EstadoSesion>().Any())
        {
            SeedEstadosSesiones(context);
            hasChanges = true;
        }

        if (hasChanges)
        {
            context.SaveChanges();
        }

        if (!context.Set<Usuario>().Any(u => u.Correo == "admin@educonnect.com") &&
            context.Set<Rol>().Any(r => r.Nombre == "Admin") &&
            context.Set<EstadoUsuario>().Any(e => e.Nombre == "APROBADO"))
        {
            SeedAdminUser(context);
        }
    }

    private static async Task SeedCatalogsAndDataAsync(DbContext context, CancellationToken cancellationToken)
    {
        var hasChanges = false;

        if (!await context.Set<Rol>().AnyAsync(cancellationToken))
        {
            SeedRoles(context);
            hasChanges = true;
        }

        if (!await context.Set<EstadoUsuario>().AnyAsync(cancellationToken))
        {
            SeedEstadosUsuarios(context);
            hasChanges = true;
        }

        if (!await context.Set<EstadoSesion>().AnyAsync(cancellationToken))
        {
            SeedEstadosSesiones(context);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Set<Usuario>().AnyAsync(u => u.Correo == "admin@educonnect.com", cancellationToken) &&
            await context.Set<Rol>().AnyAsync(r => r.Nombre == "Admin", cancellationToken) &&
            await context.Set<EstadoUsuario>().AnyAsync(e => e.Nombre == "APROBADO", cancellationToken))
        {
            await SeedAdminUserAsync(context, cancellationToken);
        }
    }

    private static void SeedRoles(DbContext context)
    {
        context.Set<Rol>().AddRange(
            new Rol { Nombre = "Admin", Descripcion = "Administrador del sistema" },
            new Rol { Nombre = "Estudiante", Descripcion = "Estudiante que solicita tutorias" },
            new Rol { Nombre = "Tutor", Descripcion = "Tutor que imparte tutorias" }
        );
    }

    private static void SeedEstadosUsuarios(DbContext context)
    {
        context.Set<EstadoUsuario>().AddRange(
            new EstadoUsuario { Nombre = "PENDIENTE", Descripcion = "Usuario pendiente de aprobacion" },
            new EstadoUsuario { Nombre = "APROBADO", Descripcion = "Usuario aprobado y activo" },
            new EstadoUsuario { Nombre = "RECHAZADO", Descripcion = "Usuario rechazado" },
            new EstadoUsuario { Nombre = "INACTIVO", Descripcion = "Usuario inactivo o dado de baja" }
        );
    }

    private static void SeedEstadosSesiones(DbContext context)
    {
        context.Set<EstadoSesion>().AddRange(
            new EstadoSesion { Nombre = "PENDIENTE", Descripcion = "Sesion pendiente de atencion" },
            new EstadoSesion { Nombre = "ATENDIDA", Descripcion = "Sesion atendida con exito" },
            new EstadoSesion { Nombre = "CANCELADA_TUTOR", Descripcion = "Sesion cancelada por el tutor" },
            new EstadoSesion { Nombre = "CANCELADA_ESTUDIANTE", Descripcion = "Sesion cancelada por el estudiante" }
        );
    }

    private static void SeedAdminUser(DbContext context)
    {
        var adminRol = context.Set<Rol>().First(r => r.Nombre == "Admin");
        var aprobadoEstado = context.Set<EstadoUsuario>().First(e => e.Nombre == "APROBADO");

        var adminUser = new Usuario
        {
            Correo = "admin@educonnect.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
            RolId = adminRol.Id,
            EstadoId = aprobadoEstado.Id,
            FechaRegistro = DateTime.UtcNow
        };

        context.Set<Usuario>().Add(adminUser);
        context.SaveChanges();

        context.Set<Administrador>().Add(new Administrador
        {
            UsuarioId = adminUser.Id,
            PasswordFase2Hash = BCrypt.Net.BCrypt.HashPassword("AdminFase2_123*")
        });
        context.SaveChanges();
    }

    private static async Task SeedAdminUserAsync(DbContext context, CancellationToken cancellationToken)
    {
        var adminRol = await context.Set<Rol>().FirstAsync(r => r.Nombre == "Admin", cancellationToken);
        var aprobadoEstado = await context.Set<EstadoUsuario>().FirstAsync(e => e.Nombre == "APROBADO", cancellationToken);

        var adminUser = new Usuario
        {
            Correo = "admin@educonnect.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
            RolId = adminRol.Id,
            EstadoId = aprobadoEstado.Id,
            FechaRegistro = DateTime.UtcNow
        };

        await context.Set<Usuario>().AddAsync(adminUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await context.Set<Administrador>().AddAsync(new Administrador
        {
            UsuarioId = adminUser.Id,
            PasswordFase2Hash = BCrypt.Net.BCrypt.HashPassword("AdminFase2_123*")
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

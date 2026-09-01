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
            ConfigureDbContext(options, builder.Configuration);
        });

        return builder;
    }

    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
        var maxRetries = 10;
        var retryDelay = TimeSpan.FromSeconds(5);

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger.LogInformation("Intentando aplicar migraciones a la base de datos (intento {Attempt}/{MaxRetries})...", attempt, maxRetries);
                var dbContext = scope.ServiceProvider.GetRequiredService<edu_connect_serviceContext>();
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Migraciones aplicadas exitosamente.");
                break;
            }
            catch (Exception ex)
            {
                if (attempt == maxRetries)
                {
                    logger.LogError(ex, "No se pudo migrar la base de datos después de {MaxRetries} intentos.", maxRetries);
                    throw;
                }

                logger.LogWarning("Base de datos no disponible aún ({Message}). Reintentando en {Delay}s...", ex.Message, retryDelay.TotalSeconds);
                await Task.Delay(retryDelay);
            }
        }
    }

    private static DbContextOptionsBuilder ConfigureDbContext(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        return options.UseSeeding((context, _) =>
                    {
                        SeedCatalogsAndData(context, configuration);
                    })
                    .UseAsyncSeeding(async (context, _, cancellationToken) =>
                    {
                        await SeedCatalogsAndDataAsync(context, configuration, cancellationToken);
                    });
    }

    private static void SeedCatalogsAndData(DbContext context, IConfiguration configuration)
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

        if (!context.Set<Materia>().Any())
        {
            SeedMaterias(context);
            hasChanges = true;
        }

        if (hasChanges)
        {
            context.SaveChanges();
        }

        var (adminEmail, adminPassword, adminPasswordFase2) = GetAdminSeedCredentials(configuration);

        if (!context.Set<Usuario>().Any(u => u.Correo == adminEmail) &&
            context.Set<Rol>().Any(r => r.Nombre == "Admin") &&
            context.Set<EstadoUsuario>().Any(e => e.Nombre == "APROBADO"))
        {
            SeedAdminUser(context, adminEmail, adminPassword, adminPasswordFase2);
        }
    }

    private static async Task SeedCatalogsAndDataAsync(DbContext context, IConfiguration configuration, CancellationToken cancellationToken)
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

        if (!await context.Set<Materia>().AnyAsync(cancellationToken))
        {
            SeedMaterias(context);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        var (adminEmail, adminPassword, adminPasswordFase2) = GetAdminSeedCredentials(configuration);

        if (!await context.Set<Usuario>().AnyAsync(u => u.Correo == adminEmail, cancellationToken) &&
            await context.Set<Rol>().AnyAsync(r => r.Nombre == "Admin", cancellationToken) &&
            await context.Set<EstadoUsuario>().AnyAsync(e => e.Nombre == "APROBADO", cancellationToken))
        {
            await SeedAdminUserAsync(context, adminEmail, adminPassword, adminPasswordFase2, cancellationToken);
        }
    }

    private static (string Email, string? Password, string? PasswordFase2) GetAdminSeedCredentials(IConfiguration configuration)
    {
        var email = configuration["ADMIN_EMAIL"]
            ?? configuration["SEED_ADMIN_EMAIL"]
            ?? configuration["AdminSeed:Email"]
            ?? configuration["AdminUser:Email"]
            ?? configuration["Seed:AdminEmail"]
            ?? Environment.GetEnvironmentVariable("ADMIN_EMAIL")
            ?? Environment.GetEnvironmentVariable("SEED_ADMIN_EMAIL")
            ?? "admin@educonnect.com";

        var password = configuration["SEED_ADMIN_PASSWORD"]
            ?? configuration["ADMIN_PASSWORD"]
            ?? configuration["AdminSeed:Password"]
            ?? configuration["AdminUser:Password"]
            ?? configuration["Seed:AdminPassword"]
            ?? Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD")
            ?? Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

        var passwordFase2 = configuration["SEED_ADMIN_PASSWORD_FASE2"]
            ?? configuration["ADMIN_PASSWORD_FASE2"]
            ?? configuration["AdminSeed:PasswordFase2"]
            ?? configuration["AdminUser:PasswordFase2"]
            ?? configuration["Seed:AdminPasswordFase2"]
            ?? Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD_FASE2")
            ?? Environment.GetEnvironmentVariable("ADMIN_PASSWORD_FASE2");

        return (email, password, passwordFase2);
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

    private static void SeedMaterias(DbContext context)
    {
        string[] materias =
        [
            "Cálculo Diferencial e Integral",
            "Álgebra Lineal",
            "Física",
            "Termodinámica",
            "Química",
            "Estructuras de Datos y Algoritmos",
            "Arquitectura de Computadoras",
            "Inteligencia Artificial y Aprendizaje Automático",
            "Mecánica de Fluidos",
            "Astronomía y Astrofísica",
            "Biología Celular y Molecular",
            "Anatomía y Fisiología Humana",
            "Genética General",
            "Microbiología e Inmunología",
            "Bioquímica Clínica",
            "Ecología y Conservación Ambiental",
            "Introducción a la Filosofía",
            "Lógica Formal y Simbólica",
            "Ética y Deontología Profesional",
            "Epistemología y Teoría del Conocimiento",
            "Filosofía Política",
            "Metafísica",
            "Literatura Universal",
            "Literatura Latinoamericana Contemporánea",
            "Lingüística General y Fonética",
            "Semiótica y Análisis del Discurso",
            "Redacción Académica y Argumentación",
            "Periodismo y Comunicación Digital",
            "Historia del Arte Clásico y Contemporáneo",
            "Teoría de la Música y Armonía",
            "Apreciación Cinematográfica",
            "Fotografía Digital y Composición",
            "Dibujo Artístico y Pintura",
            "Diseño Gráfico y Tipografía",
            "Expresión Teatral y Actuación",
            "Historia Universal Contemporánea",
            "Sociología General",
            "Antropología Cultural",
            "Psicología Cognitiva y del Aprendizaje",
            "Geografía Humana y Geopolítica",
            "Arqueología y Civilizaciones Antiguas",
            "Criminología y Victimología",
            "Microeconomía y Macroeconomía",
            "Finanzas Corporativas",
            "Contabilidad Financiera",
            "Marketing Estratégico",
            "Gestión y Dirección de Proyectos",
            "Derecho Constitucional y Derechos Humanos",
            "Inglés Técnico y Académico",
            "Francés Intermedio"
        ];

        context.Set<Materia>().AddRange(
            materias.Select(nombre => new Materia { Nombre = nombre })
        );
    }

    private static void SeedAdminUser(DbContext context, string email, string? password, string? passwordFase2)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No se puede crear el usuario administrador inicial porque no se proporcionó la contraseña. " +
                "Por favor, configure la variable de entorno 'SEED_ADMIN_PASSWORD' (o 'ADMIN_PASSWORD' / 'AdminUser:Password').");
        }

        var fase2Password = string.IsNullOrWhiteSpace(passwordFase2) ? password : passwordFase2;

        var adminRol = context.Set<Rol>().First(r => r.Nombre == "Admin");
        var aprobadoEstado = context.Set<EstadoUsuario>().First(e => e.Nombre == "APROBADO");

        var adminUser = new Usuario
        {
            Correo = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            RolId = adminRol.Id,
            EstadoId = aprobadoEstado.Id,
            FechaRegistro = DateTime.UtcNow
        };

        context.Set<Usuario>().Add(adminUser);
        context.SaveChanges();

        context.Set<Administrador>().Add(new Administrador
        {
            UsuarioId = adminUser.Id,
            PasswordFase2Hash = BCrypt.Net.BCrypt.HashPassword(fase2Password)
        });
        context.SaveChanges();
    }

    private static async Task SeedAdminUserAsync(DbContext context, string email, string? password, string? passwordFase2, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No se puede crear el usuario administrador inicial porque no se proporcionó la contraseña. " +
                "Por favor, configure la variable de entorno 'SEED_ADMIN_PASSWORD' (o 'ADMIN_PASSWORD' / 'AdminUser:Password').");
        }

        var fase2Password = string.IsNullOrWhiteSpace(passwordFase2) ? password : passwordFase2;

        var adminRol = await context.Set<Rol>().FirstAsync(r => r.Nombre == "Admin", cancellationToken);
        var aprobadoEstado = await context.Set<EstadoUsuario>().FirstAsync(e => e.Nombre == "APROBADO", cancellationToken);

        var adminUser = new Usuario
        {
            Correo = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            RolId = adminRol.Id,
            EstadoId = aprobadoEstado.Id,
            FechaRegistro = DateTime.UtcNow
        };

        await context.Set<Usuario>().AddAsync(adminUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await context.Set<Administrador>().AddAsync(new Administrador
        {
            UsuarioId = adminUser.Id,
            PasswordFase2Hash = BCrypt.Net.BCrypt.HashPassword(fase2Password)
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

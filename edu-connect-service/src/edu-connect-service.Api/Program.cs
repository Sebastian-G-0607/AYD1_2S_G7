using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Administrador;
using edu_connect_service.Api.Features.Auth;
using edu_connect_service.Api.Features.Estudiantes;
using edu_connect_service.Api.Features.Health;
using edu_connect_service.Api.Features.Materias;
using edu_connect_service.Api.Features.Tutores;
using edu_connect_service.Api.Shared.Cors;
using edu_connect_service.Api.Shared.Emails;
using edu_connect_service.Api.Shared.ErrorHandling;
using edu_connect_service.Api.Shared.OpenApi;
using edu_connect_service.Api.Shared.Authentication;
using edu_connect_service.Api.Shared.Authorization;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();

builder.Addedu_connect_serviceOracle<edu_connect_serviceContext>("edu_connect_serviceDB");

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;
    options.CombineLogs = true;
});

builder.Addedu_connect_serviceOpenApi();
builder.Addedu_connect_serviceCors();

builder.Services.AddValidation();
builder.Services.AddEmailService(builder.Configuration);
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();

var app = builder.Build();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealth();
app.MapAuth();
app.MapEstudiantes();
app.MapTutores();
app.MapAdministrador();
app.MapMaterias();

app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.Useedu_connect_serviceSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();

var connString = app.Configuration.GetConnectionString("edu_connect_serviceDB");
if (!string.IsNullOrWhiteSpace(connString))
{
    await app.MigrateDbAsync();
}
else
{
    app.Logger.LogWarning("Connection string 'edu_connect_serviceDB' is empty — skipping database migration (useful for local 2FA tests).");
}

app.Run();
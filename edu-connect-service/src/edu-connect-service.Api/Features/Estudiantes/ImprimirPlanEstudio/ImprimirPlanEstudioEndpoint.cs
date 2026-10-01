using System.Security.Claims;

namespace edu_connect_service.Api.Features.Estudiantes.ImprimirPlanEstudio;

public static class ImprimirPlanEstudioEndpoint
{
    public static void MapImprimirPlanEstudio(this IEndpointRouteBuilder app)
    {
        app.MapPost("/plan-estudio/pdf", HandleAsync)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static IResult HandleAsync(
        ImprimirPlanEstudioRequest request,
        ClaimsPrincipal user)
    {
        var rol =
            user.FindFirstValue("rol")
            ?? user.FindFirstValue(ClaimTypes.Role);

        if (!string.Equals(rol, "Estudiante", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los estudiantes pueden generar su constancia de plan de estudio."
            );
        }

        var generator = new StudyPlanPdfGenerator();
        var pdfBytes = generator.Generate(request);

        return Results.File(
            pdfBytes,
            contentType: "application/pdf",
            fileDownloadName: $"plan-estudio-{DateTime.Now:yyyyMMdd}.pdf"
        );
    }
}
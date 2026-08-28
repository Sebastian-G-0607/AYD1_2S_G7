using edu_connect_service.Api.Data;

namespace edu_connect_service.Api.Features.Health;

public static class HealthEndpoints
{
    public static void MapHealth(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/check");
        group.MapGet("/", HandleHealthAsync)
            .Produces<HealthCheckResponseDto>(StatusCodes.Status200OK)
            .Produces<HealthCheckResponseDto>(StatusCodes.Status503ServiceUnavailable);

        app.MapGet("/api/health", () => "Service is running")
            .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleHealthAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var canConnectDb = await dbContext.Database.CanConnectAsync(cancellationToken);
        var status = canConnectDb ? "Healthy" : "Unhealthy";
        var dbStatus = canConnectDb ? "Connected" : "Disconnected";

        var response = new HealthCheckResponseDto(
            Status: status,
            Service: "edu-connect-service",
            Timestamp: DateTime.UtcNow,
            Database: dbStatus
        );

        return canConnectDb
            ? Results.Ok(response)
            : Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
}

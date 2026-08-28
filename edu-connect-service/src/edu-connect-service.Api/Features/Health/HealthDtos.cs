namespace edu_connect_service.Api.Features.Health;

public record HealthCheckResponseDto(
    string Status,
    string Service,
    DateTime Timestamp,
    string Database
);

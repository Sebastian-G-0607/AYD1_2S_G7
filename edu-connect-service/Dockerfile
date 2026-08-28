# ---------- Build stage ----------
ARG DOTNET_VERSION=10.0
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-noble AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy solution and project files first to leverage Docker layer caching.
# This way, NuGet restore is only re-run when project dependencies change.
COPY edu-connect-service.slnx ./
COPY src/edu-connect-service.Api/edu-connect-service.Api.csproj src/edu-connect-service.Api/

# Restore dependencies for the API project.
RUN dotnet restore "src/edu-connect-service.Api/edu-connect-service.Api.csproj"

# Copy the full source tree and publish a Release build.
# UseAppHost=true (default) produces a native apphost binary so the runtime
# container doesn't need the `dotnet` CLI to launch the app.
COPY . .
RUN dotnet publish "src/edu-connect-service.Api/edu-connect-service.Api.csproj" \
    --configuration ${BUILD_CONFIGURATION} \
    --no-restore \
    --output /app/publish

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-noble AS runtime

WORKDIR /app

# Ensure the published files are owned by the non-root user.
COPY --from=build --chown=app:app /app/publish .

# Switch to the built-in non-root user for least privilege.
USER app

# Expose HTTP traffic. Port 8080 is the container standard and works for
# non-root users (unprivileged ports are >= 1024).
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 8080

# Health probing:
#   Endpoints available in the API:
#     /api/health  -> liveness
#     /api/check   -> readiness (database connectivity)
#   Example for docker-compose:
#     healthcheck:
#       test: ["CMD-SHELL", "wget -qO- http://127.0.0.1:8080/api/health || exit 1"]
#
# Graceful shutdown on SIGTERM (default k8s/Docker behavior).
ENTRYPOINT ["/app/edu-connect-service.Api"]

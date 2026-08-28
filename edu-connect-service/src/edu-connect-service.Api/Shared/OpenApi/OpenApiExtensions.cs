using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace edu_connect_service.Api.Shared.OpenApi;

public static class OpenApiExtensions
{
    public static IHostApplicationBuilder Addedu_connect_serviceOpenApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecurityDocumentTransformer>();
        });

        return builder;
    }

    public static WebApplication Useedu_connect_serviceSwaggerUI(this WebApplication app)
    {
        app.MapOpenApi();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "edu_connect_service API v1");
            options.EnablePersistAuthorization();
        });

        return app;
    }
}

internal sealed class BearerSecurityDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info ??= new OpenApiInfo();
        document.Info.Title = "edu_connect_service API";

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Ingrese el token JWT Bearer"
        };

        if (document.Paths is not null)
        {
            foreach (var path in document.Paths.Values)
            {
                if (path.Operations is null) continue;
                foreach (var kvp in path.Operations)
                {
                    var operation = kvp.Value;
                    operation.Security ??= [];
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
                }
            }
        }

        return Task.CompletedTask;
    }
}

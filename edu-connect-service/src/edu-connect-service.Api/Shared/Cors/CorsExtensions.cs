using Microsoft.Net.Http.Headers;

namespace edu_connect_service.Api.Shared.Cors;

public static class CorsExtensions
{
    public static IHostApplicationBuilder Addedu_connect_serviceCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        policy.AllowAnyOrigin();
                    }
                    else
                    {
                        var originsString = builder.Configuration["AllowedOrigins"] ?? string.Empty;
                        var allowedOrigins = originsString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        policy.WithOrigins(allowedOrigins);
                    }

                    policy.WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType)
                          .AllowAnyMethod();
                });
        });

        return builder;
    }
}
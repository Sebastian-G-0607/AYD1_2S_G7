namespace edu_connect_service.Api.Shared.Emails;

public static class EmailExtensions
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}


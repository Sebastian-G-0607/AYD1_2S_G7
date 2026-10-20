using edu_connect_service.Api.Shared.Emails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace edu_connect_service.Api.UnitTests.Shared.Emails;

public class EmailExtensionsTests
{
    [Fact]
    public void AddEmailService_RegistersIEmailService()
    {
        var services = new ServiceCollection();
        IConfiguration configuration = new ConfigurationBuilder().Build();

        services.AddLogging();
        services.AddSingleton(configuration);
        services.AddEmailService(configuration);

        var serviceProvider = services.BuildServiceProvider();

        var emailService = serviceProvider.GetService<IEmailService>();
        Assert.NotNull(emailService);
        Assert.IsType<EmailService>(emailService);
    }
}

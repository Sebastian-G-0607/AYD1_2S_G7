using edu_connect_service.Api.Shared.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace edu_connect_service.Api.UnitTests.Shared.Authentication;

public class AuthenticationExtensionsTests
{
    [Fact]
    public async Task AddCustomJwtAuthentication_WithValidConfig_RegistersServices()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Key", "this_is_a_very_long_secret_key_for_testing_purposes_123456"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"},
            {"Jwt:ExpirationMinutes", "60"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCustomJwtAuthentication(configuration);

        var serviceProvider = services.BuildServiceProvider();

        var jwtTokenService = serviceProvider.GetService<IJwtTokenService>();
        Assert.NotNull(jwtTokenService);
        Assert.IsType<JwtTokenService>(jwtTokenService);

        var jwtOptions = serviceProvider.GetService<IOptions<JwtOptions>>()?.Value;
        Assert.NotNull(jwtOptions);
        Assert.Equal("TestIssuer", jwtOptions.Issuer);
        Assert.Equal("TestAudience", jwtOptions.Audience);

        var authSchemeProvider = serviceProvider.GetService<IAuthenticationSchemeProvider>();
        Assert.NotNull(authSchemeProvider);
        var defaultScheme = await authSchemeProvider.GetDefaultAuthenticateSchemeAsync();
        Assert.NotNull(defaultScheme);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, defaultScheme.Name);
    }

    [Fact]
    public void AddCustomJwtAuthentication_MissingSection_ThrowsInvalidOperationException()
    {
        IConfiguration emptyConfig = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        Assert.Throws<InvalidOperationException>(() => services.AddCustomJwtAuthentication(emptyConfig));
    }
}

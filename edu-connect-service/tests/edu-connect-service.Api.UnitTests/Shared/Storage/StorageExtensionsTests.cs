using edu_connect_service.Api.Shared.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace edu_connect_service.Api.UnitTests.Shared.Storage;

public class StorageExtensionsTests
{
    [Fact]
    public void AddS3Storage_RegistersAmazonS3AndS3Service()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"S3:S3_BUCKET_NAME", "my-bucket"},
            {"S3:AWS_ACCESS_KEY_ID", "test-key"},
            {"S3:AWS_SECRET_ACCESS_KEY", "test-secret"},
            {"S3:AWS_REGION", "us-east-1"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(configuration);
        services.AddS3Storage(configuration);

        var serviceProvider = services.BuildServiceProvider();

        var s3Service = serviceProvider.GetService<IS3Service>();
        Assert.NotNull(s3Service);
        Assert.IsType<S3Service>(s3Service);
    }
}

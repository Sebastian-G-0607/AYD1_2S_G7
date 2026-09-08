using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace edu_connect_service.Api.Shared.Storage;

public static class StorageExtensions
{
    public static IServiceCollection AddS3Storage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            var accessKeyId = config["S3:AWS_ACCESS_KEY_ID"]
                ?? config["S3:ACCESS_KEY_ID"]
                ?? config["S3:AccessKeyId"]
                ?? config["S3__AWS_ACCESS_KEY_ID"]
                ?? config["S3__ACCESS_KEY_ID"]
                ?? config["S3__AccessKeyId"]
                ?? Environment.GetEnvironmentVariable("S3__AWS_ACCESS_KEY_ID")
                ?? Environment.GetEnvironmentVariable("S3__ACCESS_KEY_ID")
                ?? Environment.GetEnvironmentVariable("S3__AccessKeyId")
                ?? config["AWS_ACCESS_KEY_ID"]
                ?? config["AWS:AccessKeyId"]
                ?? config["AWS:AccessKey"]
                ?? Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");

            var secretAccessKey = config["S3:AWS_SECRET_ACCESS_KEY"]
                ?? config["S3:SECRET_ACCESS_KEY"]
                ?? config["S3:SecretAccessKey"]
                ?? config["S3__AWS_SECRET_ACCESS_KEY"]
                ?? config["S3__SECRET_ACCESS_KEY"]
                ?? config["S3__SecretAccessKey"]
                ?? Environment.GetEnvironmentVariable("S3__AWS_SECRET_ACCESS_KEY")
                ?? Environment.GetEnvironmentVariable("S3__SECRET_ACCESS_KEY")
                ?? Environment.GetEnvironmentVariable("S3__SecretAccessKey")
                ?? config["AWS_SECRET_ACCESS_KEY"]
                ?? config["AWS:SecretAccessKey"]
                ?? config["AWS:SecretKey"]
                ?? Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

            var regionName = config["S3:AWS_REGION"]
                ?? config["S3:REGION"]
                ?? config["S3:Region"]
                ?? config["S3__AWS_REGION"]
                ?? config["S3__REGION"]
                ?? config["S3__Region"]
                ?? Environment.GetEnvironmentVariable("S3__AWS_REGION")
                ?? Environment.GetEnvironmentVariable("S3__REGION")
                ?? Environment.GetEnvironmentVariable("S3__Region")
                ?? config["AWS_REGION"]
                ?? config["AWS:Region"]
                ?? Environment.GetEnvironmentVariable("AWS_REGION")
                ?? "us-east-1";

            var sessionToken = config["S3:AWS_SESSION_TOKEN"]
                ?? config["S3:SESSION_TOKEN"]
                ?? config["S3:SessionToken"]
                ?? config["S3__AWS_SESSION_TOKEN"]
                ?? config["S3__SESSION_TOKEN"]
                ?? config["S3__SessionToken"]
                ?? Environment.GetEnvironmentVariable("S3__AWS_SESSION_TOKEN")
                ?? Environment.GetEnvironmentVariable("S3__SESSION_TOKEN")
                ?? Environment.GetEnvironmentVariable("S3__SessionToken")
                ?? config["AWS_SESSION_TOKEN"]
                ?? config["AWS:SessionToken"]
                ?? Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");

            RegionEndpoint regionEndpoint;
            try
            {
                regionEndpoint = RegionEndpoint.GetBySystemName(regionName);
            }
            catch
            {
                regionEndpoint = RegionEndpoint.USEast1;
            }

            if (!string.IsNullOrWhiteSpace(accessKeyId) && !string.IsNullOrWhiteSpace(secretAccessKey))
            {
                AWSCredentials credentials = !string.IsNullOrWhiteSpace(sessionToken)
                    ? new SessionAWSCredentials(accessKeyId, secretAccessKey, sessionToken)
                    : new BasicAWSCredentials(accessKeyId, secretAccessKey);

                return new AmazonS3Client(credentials, regionEndpoint);
            }

            return new AmazonS3Client(regionEndpoint);
        });

        services.AddScoped<IS3Service, S3Service>();

        return services;
    }
}

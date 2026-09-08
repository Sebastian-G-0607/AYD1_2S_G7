using Amazon.S3;
using Amazon.S3.Model;

namespace edu_connect_service.Api.Shared.Storage;

public class S3Service(
    IAmazonS3 s3Client,
    IConfiguration configuration,
    ILogger<S3Service> logger) : IS3Service
{
    private readonly string _bucketName = configuration["S3:S3_BUCKET_NAME"]
        ?? configuration["S3:BucketName"]
        ?? configuration["S3__S3_BUCKET_NAME"]
        ?? configuration["S3__BucketName"]
        ?? Environment.GetEnvironmentVariable("S3__S3_BUCKET_NAME")
        ?? Environment.GetEnvironmentVariable("S3__BucketName")
        ?? configuration["S3_BUCKET_NAME"]
        ?? configuration["AWS:BucketName"]
        ?? Environment.GetEnvironmentVariable("S3_BUCKET_NAME")
        ?? string.Empty;

    public async Task<string> UploadImageAsync(
        IFormFile file,
        string? prefix = null,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            throw new ArgumentException("El archivo de imagen no puede estar vacío.", nameof(file));
        }

        if (string.IsNullOrWhiteSpace(_bucketName))
        {
            logger.LogError("S3_BUCKET_NAME no está configurado.");
            throw new InvalidOperationException("El nombre del bucket de S3 no está configurado en el sistema.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        var uniqueId = Guid.NewGuid().ToString("N");
        var key = string.IsNullOrWhiteSpace(prefix)
            ? $"{uniqueId}{extension.ToLowerInvariant()}"
            : $"{prefix.Trim('/')}/{uniqueId}{extension.ToLowerInvariant()}";

        using var stream = file.OpenReadStream();
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType
        };

        logger.LogInformation("Subiendo archivo a S3 bucket '{Bucket}' con key '{Key}'", _bucketName, key);
        await s3Client.PutObjectAsync(putRequest, cancellationToken);
        logger.LogInformation("Archivo subido exitosamente a S3 con key '{Key}'", key);

        return key;
    }

    public string? GeneratePresignedUrl(string? key, TimeSpan? expiration = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        if (key.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            key.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return key;
        }

        if (string.IsNullOrWhiteSpace(_bucketName))
        {
            logger.LogWarning("S3_BUCKET_NAME no está configurado al generar URL prefirmada para el key '{Key}'", key);
            return key;
        }

        try
        {
            var cleanKey = key.TrimStart('/');

            var presignedRequest = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = cleanKey,
                Expires = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromMinutes(5)),
                Verb = HttpVerb.GET
            };

            return s3Client.GetPreSignedURL(presignedRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al generar URL prefirmada para el key '{Key}'", key);
            return key;
        }
    }
}

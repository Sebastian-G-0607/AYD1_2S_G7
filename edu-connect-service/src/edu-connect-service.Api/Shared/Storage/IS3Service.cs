namespace edu_connect_service.Api.Shared.Storage;

public interface IS3Service
{
    Task<string> UploadImageAsync(IFormFile file, string? prefix = null, CancellationToken cancellationToken = default);
    string? GeneratePresignedUrl(string? key, TimeSpan? expiration = null);
    Task DeleteImageAsync(string? key, CancellationToken cancellationToken = default);
}

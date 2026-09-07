using Microsoft.AspNetCore.Http;

namespace edu_connect_service.Api.Shared.Validation;

public static class ImageFileValidator
{
    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp", ".jfif", ".svg"
    ];

    public static bool IsValidImage(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(file.ContentType) &&
            !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }
}

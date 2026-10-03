using Microsoft.AspNetCore.Http;

namespace edu_connect_service.Api.Shared.Validation;

/// <summary>
/// Valida que un archivo recibido por formulario sea realmente un documento PDF.
/// </summary>
public static class PdfFileValidator
{
    /// <summary>Tamaño máximo permitido para un PDF subido por el usuario (5 MB).</summary>
    public const long MaxFileSizeBytes = 5L * 1024 * 1024;

    private static readonly byte[] PdfSignature = "%PDF-"u8.ToArray();

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/x-pdf"
    };

    public static bool IsValidPdf(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName);
        if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(file.ContentType) && !AllowedContentTypes.Contains(file.ContentType))
        {
            return false;
        }

        return HasPdfSignature(file);
    }

    public static bool ExceedsMaxSize(IFormFile? file)
    {
        return file is not null && file.Length > MaxFileSizeBytes;
    }

    private static bool HasPdfSignature(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        Span<byte> header = stackalloc byte[5];
        var bytesRead = stream.ReadAtLeast(header, header.Length, throwOnEndOfStream: false);

        return bytesRead == PdfSignature.Length && header.SequenceEqual(PdfSignature);
    }
}

using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Http;
using Moq;

namespace edu_connect_service.Api.UnitTests.Shared.Validation;

public class PdfFileValidatorTests
{
    private static readonly byte[] ValidPdfBytes = "%PDF-1.7\n1 0 obj\n<<>>\nendobj\n"u8.ToArray();

    private static IFormFile CreateFile(string fileName, string? contentType, byte[] content, long? length = null)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns(contentType!);
        fileMock.Setup(f => f.Length).Returns(length ?? content.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(content));
        return fileMock.Object;
    }

    [Theory]
    [InlineData("carnet.pdf", "application/pdf")]
    [InlineData("CARNET.PDF", "application/pdf")]
    [InlineData("carnet.pdf", "application/x-pdf")]
    [InlineData("carnet.pdf", null)]
    public void IsValidPdf_WithValidPdf_ReturnsTrue(string fileName, string? contentType)
    {
        var file = CreateFile(fileName, contentType, ValidPdfBytes);

        var result = PdfFileValidator.IsValidPdf(file);

        Assert.True(result);
    }

    [Fact]
    public void IsValidPdf_WithNullFile_ReturnsFalse()
    {
        var result = PdfFileValidator.IsValidPdf(null);

        Assert.False(result);
    }

    [Fact]
    public void IsValidPdf_WithEmptyFile_ReturnsFalse()
    {
        var file = CreateFile("carnet.pdf", "application/pdf", []);

        var result = PdfFileValidator.IsValidPdf(file);

        Assert.False(result);
    }

    [Theory]
    [InlineData("carnet.png")]
    [InlineData("carnet.jpg")]
    [InlineData("carnet.docx")]
    [InlineData("carnet.exe")]
    [InlineData("carnet.pdf.exe")]
    [InlineData("carnet")]
    public void IsValidPdf_WithNonPdfExtension_ReturnsFalse(string fileName)
    {
        var file = CreateFile(fileName, "application/pdf", ValidPdfBytes);

        var result = PdfFileValidator.IsValidPdf(file);

        Assert.False(result);
    }

    [Theory]
    [InlineData("image/png")]
    [InlineData("text/plain")]
    [InlineData("application/octet-stream")]
    public void IsValidPdf_WithNonPdfContentType_ReturnsFalse(string contentType)
    {
        var file = CreateFile("carnet.pdf", contentType, ValidPdfBytes);

        var result = PdfFileValidator.IsValidPdf(file);

        Assert.False(result);
    }

    [Fact]
    public void IsValidPdf_WithPdfExtensionButWithoutPdfSignature_ReturnsFalse()
    {
        var notReallyAPdf = "Este archivo solo fue renombrado a .pdf"u8.ToArray();
        var file = CreateFile("carnet.pdf", "application/pdf", notReallyAPdf);

        var result = PdfFileValidator.IsValidPdf(file);

        Assert.False(result);
    }

    [Fact]
    public void IsValidPdf_WithFileShorterThanSignature_ReturnsFalse()
    {
        var file = CreateFile("carnet.pdf", "application/pdf", "%PD"u8.ToArray());

        var result = PdfFileValidator.IsValidPdf(file);

        Assert.False(result);
    }

    [Fact]
    public void ExceedsMaxSize_WithFileExactlyAtLimit_ReturnsFalse()
    {
        var file = CreateFile("carnet.pdf", "application/pdf", ValidPdfBytes, PdfFileValidator.MaxFileSizeBytes);

        var result = PdfFileValidator.ExceedsMaxSize(file);

        Assert.False(result);
    }

    [Fact]
    public void ExceedsMaxSize_WithFileOverLimit_ReturnsTrue()
    {
        var file = CreateFile("carnet.pdf", "application/pdf", ValidPdfBytes, PdfFileValidator.MaxFileSizeBytes + 1);

        var result = PdfFileValidator.ExceedsMaxSize(file);

        Assert.True(result);
    }

    [Fact]
    public void ExceedsMaxSize_WithNullFile_ReturnsFalse()
    {
        var result = PdfFileValidator.ExceedsMaxSize(null);

        Assert.False(result);
    }
}

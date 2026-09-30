using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Http;
using Moq;

namespace edu_connect_service.Api.UnitTests.Shared.Validation;

public class ImageFileValidatorTests
{
    [Theory]
    [InlineData("avatar.jpg", "image/jpeg")]
    [InlineData("photo.jpeg", "image/jpeg")]
    [InlineData("picture.png", "image/png")]
    [InlineData("graphic.webp", "image/webp")]
    [InlineData("anim.gif", "image/gif")]
    [InlineData("bitmap.bmp", "image/bmp")]
    [InlineData("image.jfif", "image/jpeg")]
    [InlineData("vector.svg", "image/svg+xml")]
    public void IsValidImage_WithValidExtensionAndContentType_ReturnsTrue(string fileName, string contentType)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns(contentType);

        var result = ImageFileValidator.IsValidImage(fileMock.Object);

        Assert.True(result);
    }

    [Fact]
    public void IsValidImage_WithValidExtensionAndNullContentType_ReturnsTrue()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.FileName).Returns("photo.png");
        fileMock.Setup(f => f.ContentType).Returns((string)null!);

        var result = ImageFileValidator.IsValidImage(fileMock.Object);

        Assert.True(result);
    }

    [Fact]
    public void IsValidImage_WithNullFile_ReturnsFalse()
    {
        var result = ImageFileValidator.IsValidImage(null);

        Assert.False(result);
    }

    [Fact]
    public void IsValidImage_WithZeroLength_ReturnsFalse()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);
        fileMock.Setup(f => f.FileName).Returns("photo.png");

        var result = ImageFileValidator.IsValidImage(fileMock.Object);

        Assert.False(result);
    }

    [Theory]
    [InlineData("document.pdf")]
    [InlineData("script.exe")]
    [InlineData("data.txt")]
    [InlineData("noextension")]
    public void IsValidImage_WithInvalidExtension_ReturnsFalse(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns("image/png");

        var result = ImageFileValidator.IsValidImage(fileMock.Object);

        Assert.False(result);
    }

    [Fact]
    public void IsValidImage_WithNonImageContentType_ReturnsFalse()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.FileName).Returns("photo.png");
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");

        var result = ImageFileValidator.IsValidImage(fileMock.Object);

        Assert.False(result);
    }
}

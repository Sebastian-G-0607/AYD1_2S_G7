using Amazon.S3;
using Amazon.S3.Model;
using edu_connect_service.Api.Shared.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace edu_connect_service.Api.UnitTests.Shared.Storage;

public class S3ServiceTests
{
    private readonly Mock<IAmazonS3> _s3ClientMock = new();
    private readonly Mock<ILogger<S3Service>> _loggerMock = new();

    private IConfiguration CreateConfiguration(string? bucketName = "test-bucket")
    {
        var settings = new Dictionary<string, string?>();
        if (bucketName is not null)
        {
            settings["S3:S3_BUCKET_NAME"] = bucketName;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Fact]
    public async Task UploadImageAsync_WithNullFile_ThrowsArgumentException()
    {
        var config = CreateConfiguration();
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => service.UploadImageAsync(null!));
    }

    [Fact]
    public async Task UploadImageAsync_WithEmptyFile_ThrowsArgumentException()
    {
        var config = CreateConfiguration();
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        await Assert.ThrowsAsync<ArgumentException>(() => service.UploadImageAsync(fileMock.Object));
    }

    [Fact]
    public async Task UploadImageAsync_WithoutBucketName_ThrowsInvalidOperationException()
    {
        var config = CreateConfiguration(null);
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.FileName).Returns("photo.png");

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadImageAsync(fileMock.Object));
    }

    [Fact]
    public async Task UploadImageAsync_WithValidFileAndPrefix_UploadsObjectAndReturnsKey()
    {
        var config = CreateConfiguration("test-bucket");
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.FileName).Returns("avatar.png");
        fileMock.Setup(f => f.ContentType).Returns("image/png");
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream([1, 2, 3]));

        _s3ClientMock
            .Setup(c => c.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse());

        var key = await service.UploadImageAsync(fileMock.Object, "avatars");

        Assert.NotNull(key);
        Assert.StartsWith("avatars/", key);
        Assert.EndsWith(".png", key);

        _s3ClientMock.Verify(c => c.PutObjectAsync(It.Is<PutObjectRequest>(r =>
            r.BucketName == "test-bucket" &&
            r.Key == key &&
            r.ContentType == "image/png"
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void GeneratePresignedUrl_WithNullOrWhitespaceKey_ReturnsNull()
    {
        var config = CreateConfiguration("test-bucket");
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);

        var result = service.GeneratePresignedUrl(null);
        Assert.Null(result);

        var resultEmpty = service.GeneratePresignedUrl("   ");
        Assert.Null(resultEmpty);
    }

    [Fact]
    public void GeneratePresignedUrl_WithHttpOrHttpsUrl_ReturnsSameUrl()
    {
        var config = CreateConfiguration("test-bucket");
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);

        var httpUrl = "http://example.com/image.jpg";
        var httpsUrl = "https://example.com/image.jpg";

        Assert.Equal(httpUrl, service.GeneratePresignedUrl(httpUrl));
        Assert.Equal(httpsUrl, service.GeneratePresignedUrl(httpsUrl));
    }

    [Fact]
    public void GeneratePresignedUrl_WithMissingBucket_ReturnsKey()
    {
        var config = CreateConfiguration(null);
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);

        var key = "images/photo.png";
        var result = service.GeneratePresignedUrl(key);

        Assert.Equal(key, result);
    }

    [Fact]
    public void GeneratePresignedUrl_WithValidKey_CallsS3Client()
    {
        var config = CreateConfiguration("test-bucket");
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);

        _s3ClientMock
            .Setup(c => c.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
            .Returns("https://s3.amazonaws.com/presigned-url");

        var result = service.GeneratePresignedUrl("images/photo.png");

        Assert.Equal("https://s3.amazonaws.com/presigned-url", result);
        _s3ClientMock.Verify(c => c.GetPreSignedURL(It.Is<GetPreSignedUrlRequest>(r =>
            r.BucketName == "test-bucket" &&
            r.Key == "images/photo.png"
        )), Times.Once);
    }

    [Fact]
    public async Task DeleteImageAsync_WithNullOrEmptyKey_DoesNothing()
    {
        var config = CreateConfiguration("test-bucket");
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);

        await service.DeleteImageAsync(null);
        await service.DeleteImageAsync("");

        _s3ClientMock.Verify(c => c.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteImageAsync_WithValidKey_DeletesObject()
    {
        var config = CreateConfiguration("test-bucket");
        var service = new S3Service(_s3ClientMock.Object, config, _loggerMock.Object);

        _s3ClientMock
            .Setup(c => c.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectResponse());

        await service.DeleteImageAsync("/images/photo.png");

        _s3ClientMock.Verify(c => c.DeleteObjectAsync(It.Is<DeleteObjectRequest>(r =>
            r.BucketName == "test-bucket" &&
            r.Key == "images/photo.png"
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}

using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Storage;
using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace edu_connect_service.Api.UnitTests.Features.Estudiantes.RegistrarEstudiante;

/// <summary>
/// HU-22: Registro de estudiante con fotografía y carnet escaneado en PDF.
/// </summary>
public class RegistrarEstudianteEndpointTests
{
    private const string FotoKey = "estudiantes/foto-key.png";
    private const string CarnetKey = "estudiantes/carnets/carnet-key.pdf";

    private static readonly byte[] ValidPdfBytes = "%PDF-1.7\n1 0 obj\n<<>>\nendobj\n"u8.ToArray();
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private readonly Mock<IS3Service> _s3ServiceMock = new();

    private static edu_connect_serviceContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(databaseName: "RegistrarEstudianteEndpointTests_" + Guid.NewGuid())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var db = new edu_connect_serviceContext(options);
        db.Roles.Add(new Rol { Id = 1, Nombre = "Estudiante" });
        db.EstadosUsuarios.Add(new EstadoUsuario { Id = 1, Nombre = "PENDIENTE" });
        db.SaveChanges();
        return db;
    }

    private static IFormFile CreateFile(string fileName, string contentType, byte[] content, long? length = null)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.Length).Returns(length ?? content.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(content));
        return fileMock.Object;
    }

    private static IFormFile ValidFoto() => CreateFile("foto.png", "image/png", PngBytes);

    private static IFormFile ValidCarnetPdf() => CreateFile("carnet.pdf", "application/pdf", ValidPdfBytes);

    private static RegistrarEstudianteRequestDto CreateRequest(IFormFile? fotografia, IFormFile? documentoCarnet)
    {
        return new RegistrarEstudianteRequestDto
        {
            Nombre = "Carlos",
            Apellido = "Mendoza",
            Carnet = "202200123",
            Genero = "masculino",
            Direccion = "Zona 1, Ciudad de Guatemala",
            Telefono = "55551234",
            FechaNacimiento = new DateOnly(2000, 3, 15),
            Correo = "carlos.mendoza@educonnect.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            Fotografia = fotografia,
            DocumentoCarnet = documentoCarnet
        };
    }

    private void SetupSuccessfulStorage()
    {
        _s3ServiceMock
            .Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>(), "estudiantes", It.IsAny<CancellationToken>()))
            .ReturnsAsync(FotoKey);
        _s3ServiceMock
            .Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>(), "estudiantes/carnets", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CarnetKey);
        _s3ServiceMock
            .Setup(s => s.GeneratePresignedUrl(It.IsAny<string?>(), It.IsAny<TimeSpan?>()))
            .Returns((string? key, TimeSpan? _) => key is null ? null : $"https://signed.test/{key}");
    }

    private void VerifyNothingWasUploaded()
    {
        _s3ServiceMock.Verify(
            s => s.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _s3ServiceMock.Verify(
            s => s.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithoutFotografia_Returns400AndDoesNotUploadFiles()
    {
        using var db = CreateInMemoryDbContext();
        var request = CreateRequest(fotografia: null, documentoCarnet: ValidCarnetPdf());

        var result = await RegistrarEstudianteEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Fotografía obligatoria", problem.ProblemDetails.Title);
        Assert.Empty(db.Estudiantes);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithoutDocumentoCarnet_Returns400AndDoesNotUploadFiles()
    {
        using var db = CreateInMemoryDbContext();
        var request = CreateRequest(fotografia: ValidFoto(), documentoCarnet: null);

        var result = await RegistrarEstudianteEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Documento PDF obligatorio", problem.ProblemDetails.Title);
        Assert.Empty(db.Estudiantes);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithCarnetThatIsNotPdf_Returns400AndDoesNotUploadFiles()
    {
        using var db = CreateInMemoryDbContext();
        var carnetEnImagen = CreateFile("carnet.png", "image/png", PngBytes);
        var request = CreateRequest(fotografia: ValidFoto(), documentoCarnet: carnetEnImagen);

        var result = await RegistrarEstudianteEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Documento PDF inválido", problem.ProblemDetails.Title);
        Assert.Empty(db.Estudiantes);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithFileRenamedToPdf_Returns400()
    {
        using var db = CreateInMemoryDbContext();
        var falsoPdf = CreateFile("carnet.pdf", "application/pdf", "contenido que no es un PDF"u8.ToArray());
        var request = CreateRequest(fotografia: ValidFoto(), documentoCarnet: falsoPdf);

        var result = await RegistrarEstudianteEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Documento PDF inválido", problem.ProblemDetails.Title);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithCarnetPdfOverMaxSize_Returns400()
    {
        using var db = CreateInMemoryDbContext();
        var pdfGrande = CreateFile("carnet.pdf", "application/pdf", ValidPdfBytes, PdfFileValidator.MaxFileSizeBytes + 1);
        var request = CreateRequest(fotografia: ValidFoto(), documentoCarnet: pdfGrande);

        var result = await RegistrarEstudianteEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Documento PDF demasiado grande", problem.ProblemDetails.Title);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithValidFiles_Returns201AndStoresBothFileKeys()
    {
        using var db = CreateInMemoryDbContext();
        SetupSuccessfulStorage();
        var request = CreateRequest(fotografia: ValidFoto(), documentoCarnet: ValidCarnetPdf());

        var result = await RegistrarEstudianteEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var status = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
        Assert.Equal(StatusCodes.Status201Created, status.StatusCode);

        var body = Assert.IsType<EstudianteResponseDto>(Assert.IsAssignableFrom<IValueHttpResult>(result).Value);
        Assert.Equal($"https://signed.test/{CarnetKey}", body.DocumentoCarnetUrl);
        Assert.Equal("PENDIENTE", body.Estado);

        var estudiante = Assert.Single(db.Estudiantes);
        Assert.Equal(FotoKey, estudiante.FotografiaUrl);
        Assert.Equal(CarnetKey, estudiante.DocumentoCarnetUrl);
    }

    [Fact]
    public async Task HandleAsync_WhenCarnetUploadFails_DeletesUploadedPhotoAndReturns500()
    {
        using var db = CreateInMemoryDbContext();
        _s3ServiceMock
            .Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>(), "estudiantes", It.IsAny<CancellationToken>()))
            .ReturnsAsync(FotoKey);
        _s3ServiceMock
            .Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("S3 no disponible"));
        var request = CreateRequest(fotografia: ValidFoto(), documentoCarnet: ValidCarnetPdf());

        var result = await RegistrarEstudianteEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problem.StatusCode);
        Assert.Empty(db.Estudiantes);
        _s3ServiceMock.Verify(s => s.DeleteImageAsync(FotoKey, It.IsAny<CancellationToken>()), Times.Once);
    }
}

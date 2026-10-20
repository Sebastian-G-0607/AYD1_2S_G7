using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Tutores.RegistrarTutor;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Storage;
using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace edu_connect_service.Api.UnitTests.Features.Tutores.RegistrarTutor;

public class RegistrarTutorEndpointTests
{
    private const string FotoKey = "tutores/foto-key.png";
    private const string CvKey = "tutores/cv/cv-key.pdf";

    private static readonly byte[] ValidPdfBytes = "%PDF-1.7\n1 0 obj\n<<>>\nendobj\n"u8.ToArray();
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private readonly Mock<IS3Service> _s3ServiceMock = new();

    private static edu_connect_serviceContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(databaseName: "RegistrarTutorEndpointTests_" + Guid.NewGuid())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var db = new edu_connect_serviceContext(options);
        db.Roles.Add(new Rol { Id = 1, Nombre = "Tutor" });
        db.EstadosUsuarios.Add(new EstadoUsuario { Id = 1, Nombre = "PENDIENTE" });
        db.Materias.Add(new Materia { Id = 1, Nombre = "Física" });
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

    private static IFormFile ValidCvPdf() => CreateFile("cv.pdf", "application/pdf", ValidPdfBytes);

    private static RegistrarTutorRequestDto CreateRequest(IFormFile? documentoCv)
    {
        return new RegistrarTutorRequestDto
        {
            Nombre = "Sofia",
            Apellido = "Ramirez",
            CarnetId = "201800987",
            NumeroIdentificacion = "2589631470101",
            Genero = "femenino",
            Direccion = "Zona 10, Ciudad de Guatemala",
            Telefono = "55559876",
            FechaNacimiento = new DateOnly(1995, 7, 20),
            Fotografia = ValidFoto(),
            DocumentoCv = documentoCv,
            DireccionTutoria = "Edificio T-3, Aula 201",
            AnioInicio = 2018,
            Universidad = "Universidad de San Carlos de Guatemala",
            Correo = "sofia.ramirez@educonnect.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            MateriasIds = [1],
            DiasAtencion = [1, 2, 3],
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(17, 0)
        };
    }

    private void SetupSuccessfulStorage()
    {
        _s3ServiceMock
            .Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>(), "tutores", It.IsAny<CancellationToken>()))
            .ReturnsAsync(FotoKey);
        _s3ServiceMock
            .Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>(), "tutores/cv", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CvKey);
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
    public async Task HandleAsync_WithoutDocumentoCv_Returns400AndDoesNotUploadFiles()
    {
        using var db = CreateInMemoryDbContext();
        var request = CreateRequest(documentoCv: null);

        var result = await RegistrarTutorEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Currículum PDF obligatorio", problem.ProblemDetails.Title);
        Assert.Empty(db.Tutores);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithEmptyDocumentoCv_Returns400()
    {
        using var db = CreateInMemoryDbContext();
        var request = CreateRequest(CreateFile("cv.pdf", "application/pdf", []));

        var result = await RegistrarTutorEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal("Currículum PDF obligatorio", problem.ProblemDetails.Title);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithCvThatIsNotPdf_Returns400AndDoesNotUploadFiles()
    {
        using var db = CreateInMemoryDbContext();
        var cvEnImagen = CreateFile("cv.png", "image/png", PngBytes);
        var request = CreateRequest(cvEnImagen);

        var result = await RegistrarTutorEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Currículum PDF inválido", problem.ProblemDetails.Title);
        Assert.Empty(db.Tutores);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithFileRenamedToPdf_Returns400()
    {
        using var db = CreateInMemoryDbContext();
        var falsoPdf = CreateFile("cv.pdf", "application/pdf", "contenido que no es un PDF"u8.ToArray());
        var request = CreateRequest(falsoPdf);

        var result = await RegistrarTutorEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Currículum PDF inválido", problem.ProblemDetails.Title);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithCvPdfOverMaxSize_Returns400()
    {
        using var db = CreateInMemoryDbContext();
        var pdfGrande = CreateFile("cv.pdf", "application/pdf", ValidPdfBytes, PdfFileValidator.MaxFileSizeBytes + 1);
        var request = CreateRequest(pdfGrande);

        var result = await RegistrarTutorEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Currículum PDF demasiado grande", problem.ProblemDetails.Title);
        VerifyNothingWasUploaded();
    }

    [Fact]
    public async Task HandleAsync_WithValidFiles_Returns201AndStoresBothFileKeys()
    {
        using var db = CreateInMemoryDbContext();
        SetupSuccessfulStorage();
        var request = CreateRequest(ValidCvPdf());

        var result = await RegistrarTutorEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var status = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
        Assert.Equal(StatusCodes.Status201Created, status.StatusCode);

        var body = Assert.IsType<TutorResponseDto>(Assert.IsAssignableFrom<IValueHttpResult>(result).Value);
        Assert.Equal($"https://signed.test/{CvKey}", body.DocumentoCvUrl);
        Assert.Equal("PENDIENTE", body.Estado);

        var tutor = Assert.Single(db.Tutores);
        Assert.Equal(FotoKey, tutor.FotografiaUrl);
        Assert.Equal(CvKey, tutor.DocumentoCvUrl);
    }

    [Fact]
    public async Task HandleAsync_WhenCvUploadFails_DeletesUploadedPhotoAndReturns500()
    {
        using var db = CreateInMemoryDbContext();
        _s3ServiceMock
            .Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>(), "tutores", It.IsAny<CancellationToken>()))
            .ReturnsAsync(FotoKey);
        _s3ServiceMock
            .Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("S3 no disponible"));
        var request = CreateRequest(ValidCvPdf());

        var result = await RegistrarTutorEndpoint.HandleAsync(
            request, db, _s3ServiceMock.Object, CancellationToken.None);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problem.StatusCode);
        Assert.Empty(db.Tutores);
        _s3ServiceMock.Verify(s => s.DeleteImageAsync(FotoKey, It.IsAny<CancellationToken>()), Times.Once);
    }
}

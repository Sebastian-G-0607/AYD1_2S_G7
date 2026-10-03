using edu_connect_service.Api.Features.Estudiantes.ImprimirPlanEstudio;

namespace edu_connect_service.Api.UnitTests.Features.Estudiantes.ImprimirPlanEstudio;

public class StudyPlanPdfGeneratorTests
{
    static StudyPlanPdfGeneratorTests()
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    }
    private static ImprimirPlanEstudioRequest CrearRequestValido(int cantidadRecursos = 2)
    {
        var recursos = new List<RecursoRecomendadoRequest>();

        for (var i = 0; i < cantidadRecursos; i++)
        {
            recursos.Add(new RecursoRecomendadoRequest(
                $"Recurso {i + 1}",
                "PDF",
                $"Descripción de uso {i + 1}"
            ));
        }

        return new ImprimirPlanEstudioRequest(
            "2026-09-24",
            "Ana Lucía Pérez",
            "TUT-00457",
            "Matemática, Cálculo Diferencial",
            "Dificultad con derivadas",
            recursos
        );
    }

    [Fact]
    public void Generate_WithValidRequest_ReturnsNonEmptyByteArray()
    {
        var generator = new StudyPlanPdfGenerator();
        var request = CrearRequestValido();

        var result = generator.Generate(request);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Generate_WithValidRequest_ReturnsValidPdfHeader()
    {
        var generator = new StudyPlanPdfGenerator();
        var request = CrearRequestValido();

        var result = generator.Generate(request);

        var header = System.Text.Encoding.ASCII.GetString(result, 0, 4);
        Assert.Equal("%PDF", header);
    }

    [Fact]
    public void Generate_WithNoResources_DoesNotThrow()
    {
        var generator = new StudyPlanPdfGenerator();
        var request = CrearRequestValido(cantidadRecursos: 0);

        var exception = Record.Exception(() => generator.Generate(request));

        Assert.Null(exception);
    }

    [Fact]
    public void Generate_WithMultipleResources_ReturnsLargerDocumentThanSingleResource()
    {
        var generator = new StudyPlanPdfGenerator();
        var requestConUnRecurso = CrearRequestValido(cantidadRecursos: 1);
        var requestConCincoRecursos = CrearRequestValido(cantidadRecursos: 5);

        var resultUno = generator.Generate(requestConUnRecurso);
        var resultCinco = generator.Generate(requestConCincoRecursos);

        Assert.True(resultCinco.Length > resultUno.Length);
    }

    [Fact]
    public void Generate_WithLongDifficultyText_DoesNotThrow()
    {
        var generator = new StudyPlanPdfGenerator();
        var textoLargo = string.Join(" ", Enumerable.Repeat("Texto de prueba repetido.", 50));
        var request = CrearRequestValido() with { DificultadesIdentificadas = textoLargo };

        var exception = Record.Exception(() => generator.Generate(request));

        Assert.Null(exception);
    }
}
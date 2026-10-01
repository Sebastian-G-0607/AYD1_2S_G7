using edu_connect_service.Api.Shared.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace edu_connect_service.Api.Features.Estudiantes.ImprimirPlanEstudio;

public sealed class StudyPlanPdfGenerator : IPdfGenerator<ImprimirPlanEstudioRequest>
{
    public byte[] Generate(ImprimirPlanEstudioRequest model)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontFamily(PdfDocumentDefaults.FontFamily).FontSize(10));

                page.Header().Element(c => ComposeHeader(c));
                page.Content().Element(c => ComposeBody(c, model));
                page.Footer().Element(c => ComposeFooter(c, model));
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text(PdfDocumentDefaults.NombrePlataforma)
                .FontSize(20).Bold().FontColor(PdfDocumentDefaults.ColorPrimario);

            column.Item().Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy}")
                .FontSize(9).FontColor(PdfDocumentDefaults.ColorTextoSecundario);

            column.Item().Text($"Teléfono de contacto: {PdfDocumentDefaults.TelefonoContacto}")
                .FontSize(9).FontColor(PdfDocumentDefaults.ColorTextoSecundario);

            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(PdfDocumentDefaults.ColorBorde);
        });
    }

    private static void ComposeBody(IContainer container, ImprimirPlanEstudioRequest model)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(15);

            column.Item().Text("Constancia de Plan de Estudio")
                .FontSize(16).Bold();

            column.Item().Text($"Dificultades identificadas: {model.DificultadesIdentificadas}")
                .FontSize(10);

            column.Item().Text("Recursos recomendados").FontSize(12).Bold();

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(5);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellHeaderStyle).Text("Nombre");
                    header.Cell().Element(CellHeaderStyle).Text("Tipo");
                    header.Cell().Element(CellHeaderStyle).Text("Descripción de uso");
                });

                foreach (var recurso in model.Recursos)
                {
                    table.Cell().Element(CellStyle).Text(recurso.Nombre);
                    table.Cell().Element(CellStyle).Text(recurso.Tipo);
                    table.Cell().Element(CellStyle).Text(recurso.DescripcionUso);
                }
            });
        });

        static IContainer CellHeaderStyle(IContainer c) =>
            c.Background(PdfDocumentDefaults.ColorBorde).Padding(6).DefaultTextStyle(x => x.Bold());

        static IContainer CellStyle(IContainer c) =>
            c.BorderBottom(1).BorderColor(PdfDocumentDefaults.ColorBorde).Padding(6);
    }

    private static void ComposeFooter(IContainer container, ImprimirPlanEstudioRequest model)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(PdfDocumentDefaults.ColorBorde);

            column.Item().PaddingTop(8).Text(text =>
            {
                text.Span("Tutor responsable: ").Bold();
                text.Span(model.TutorNombre);
            });

            column.Item().Text(text =>
            {
                text.Span("Especialidad: ").Bold();
                text.Span(model.TutorEspecialidad);
            });

            column.Item().Text(text =>
            {
                text.Span("No. de identificación: ").Bold();
                text.Span(model.TutorIdentificacion);
            });
        });
    }
}
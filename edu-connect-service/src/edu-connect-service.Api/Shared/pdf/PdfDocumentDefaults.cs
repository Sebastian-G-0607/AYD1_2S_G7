using QuestPDF.Infrastructure;

namespace edu_connect_service.Api.Shared.Pdf;

/// <summary>
/// Constantes de estilo compartidas entre todos los documentos PDF
/// generados por la plataforma (plan de estudio, reportes, etc.).
/// </summary>
public static class PdfDocumentDefaults
{
    public const string NombrePlataforma = "EduConnect";
    public const string TelefonoContacto = "+502 2345-6789";

    public static readonly string FontFamily = "Arial";

    public static readonly Color ColorPrimario = Color.FromHex("#2563EB");
    public static readonly Color ColorTextoSecundario = Color.FromHex("#6B7280");
    public static readonly Color ColorBorde = Color.FromHex("#E5E7EB");
}
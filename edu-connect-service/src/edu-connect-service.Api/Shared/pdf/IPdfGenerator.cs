namespace edu_connect_service.Api.Shared.Pdf;

/// <summary>
/// Contrato genérico para generación de documentos PDF.
/// Cualquier feature que necesite un PDF (plan de estudio, reportes, etc.)
/// puede registrar su propio generador implementando esta interfaz.
/// </summary>
public interface IPdfGenerator<in TModel>
{
    byte[] Generate(TModel model);
}
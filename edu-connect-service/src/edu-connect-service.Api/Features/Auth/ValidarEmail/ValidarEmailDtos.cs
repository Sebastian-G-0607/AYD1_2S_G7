using System.Text.Json.Serialization;

namespace edu_connect_service.Api.Features.Auth.ValidarEmail;

public record ValidarEmailRequestDto(
    [property: JsonPropertyName("token")] string? Token,
    [property: JsonPropertyName("codigo")] string? Codigo
)
{
    public string ObtenerCodigo() =>
        !string.IsNullOrWhiteSpace(Token) ? Token.Trim() : (Codigo?.Trim() ?? string.Empty);
}

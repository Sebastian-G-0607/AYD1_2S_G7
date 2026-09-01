namespace edu_connect_service.Api.Shared.Validation;

public static class GeneroValidator
{
    public static bool TryNormalize(string? genero, out string normalizedGenero)
    {
        normalizedGenero = string.Empty;

        if (string.IsNullOrWhiteSpace(genero))
        {
            return false;
        }

        var lower = genero.Trim().ToLowerInvariant();

        if (lower is "m" or "masculino")
        {
            normalizedGenero = "masculino";
            return true;
        }

        if (lower is "f" or "femenino")
        {
            normalizedGenero = "femenino";
            return true;
        }

        return false;
    }
}

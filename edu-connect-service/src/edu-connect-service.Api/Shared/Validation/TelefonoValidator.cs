using System.Text.RegularExpressions;

namespace edu_connect_service.Api.Shared.Validation;

public static partial class TelefonoValidator
{
    [GeneratedRegex(@"^\d{8}$")]
    private static partial Regex TelefonoRegex();

    public static bool IsValid(string? telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
        {
            return false;
        }

        return TelefonoRegex().IsMatch(telefono.Trim());
    }
}

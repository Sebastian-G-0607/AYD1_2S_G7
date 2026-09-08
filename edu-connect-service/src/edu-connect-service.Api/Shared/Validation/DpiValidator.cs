using System.Text.RegularExpressions;

namespace edu_connect_service.Api.Shared.Validation;

public static partial class DpiValidator
{
    [GeneratedRegex(@"^\d{13}$")]
    private static partial Regex DpiRegex();

    public static bool IsValid(string? dpi)
    {
        if (string.IsNullOrWhiteSpace(dpi))
        {
            return false;
        }

        return DpiRegex().IsMatch(dpi.Trim());
    }
}

using System.Text.RegularExpressions;

namespace edu_connect_service.Api.Shared.Validation;

public static partial class CarnetValidator
{
    [GeneratedRegex(@"^\d{6,10}$")]
    private static partial Regex CarnetRegex();

    public static bool IsValid(string? carnet)
    {
        if (string.IsNullOrWhiteSpace(carnet))
        {
            return false;
        }

        return CarnetRegex().IsMatch(carnet.Trim());
    }
}

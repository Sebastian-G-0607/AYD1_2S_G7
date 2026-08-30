using System.Text.RegularExpressions;

namespace edu_connect_service.Api.Shared.Validation;

public static partial class PasswordValidator
{
    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")]
    private static partial Regex PasswordRegex();

    public static bool IsValid(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        return PasswordRegex().IsMatch(password);
    }
}

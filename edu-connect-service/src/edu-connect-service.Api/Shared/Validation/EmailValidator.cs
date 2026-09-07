using System.Net.Mail;
using System.Text.RegularExpressions;

namespace edu_connect_service.Api.Shared.Validation;

public static partial class EmailValidator
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    public static bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var trimmed = email.Trim();
        if (!EmailRegex().IsMatch(trimmed))
        {
            return false;
        }

        return MailAddress.TryCreate(trimmed, out _);
    }
}

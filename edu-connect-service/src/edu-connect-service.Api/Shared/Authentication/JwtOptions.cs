using System.ComponentModel.DataAnnotations;

namespace edu_connect_service.Api.Shared.Authentication;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    [MinLength(32)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 10080)]
    public int ExpirationMinutes { get; set; } = 120;
}

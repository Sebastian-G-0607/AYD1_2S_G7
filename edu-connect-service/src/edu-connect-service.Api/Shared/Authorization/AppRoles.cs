namespace edu_connect_service.Api.Shared.Authorization;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Administrador = Admin;
    public const string Estudiante = "Estudiante";
    public const string Tutor = "Tutor";

    public static readonly IReadOnlyList<string> All = [Admin, Estudiante, Tutor];

    public static bool IsValidRole(string role) => All.Contains(role);
}

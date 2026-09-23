namespace InnoAppointmentsApi.Constants;

public static class AuthPolicies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireStaff = "RequireStaff";
    public const string RequirePatientOrAdmin = "RequirePatientOrAdmin";
    public const string RequireAllRoles = "RequireAllRoles";
}
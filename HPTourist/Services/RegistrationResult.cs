using HPTourist.Models;

namespace HPTourist.Services;

public sealed record RegistrationResult(bool Success, Patient? Patient, string? ErrorMessage)
{
    public static RegistrationResult Ok(Patient patient) => new(true, patient, null);

    public static RegistrationResult Fail(string message) => new(false, null, message);
}

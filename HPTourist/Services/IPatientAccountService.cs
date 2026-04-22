using System.Security.Claims;
using HPTourist.Models;

namespace HPTourist.Services;

public interface IPatientAccountService
{
    Task<RegistrationResult> RegisterAsync(PatientRegistrationForm form, CancellationToken ct = default);

    Task<Patient?> ValidateCredentialsAsync(PatientLoginForm form, CancellationToken ct = default);

    ClaimsPrincipal BuildPrincipal(Patient patient);
}

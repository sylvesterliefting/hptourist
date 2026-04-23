using System.Security.Claims;
using HPTourist.Data.DTOs;
using HPTourist.Data.Models;

namespace HPTourist.Services;

public interface IPatientAccountService
{
    // High-level operations — the only ones the Razor pages need to know about.
    Task<AccountResult> AddPatientInformationAsync(PatientInformationInputForm form, CancellationToken ct = default);

    Task<AccountResult> RegisterAsync(PatientRegistrationForm form, CancellationToken ct = default);

    Task<AccountResult> LoginAsync(PatientLoginForm form, CancellationToken ct = default);

    Task LogoutAsync();

    // Atomic operations — composed by the high-level methods above, exposed for reuse/testing.
    Task<User?> ValidateCredentialsAsync(PatientLoginForm form, CancellationToken ct = default);

    ClaimsPrincipal BuildPrincipal(User user);

    Task SignInAsync(ClaimsPrincipal principal);

    Task SignOutAsync();
}

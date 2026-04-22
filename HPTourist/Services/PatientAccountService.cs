using System.Security.Claims;
using HPTourist.Database;
using HPTourist.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Services;

public sealed class PatientAccountService(
    DatabaseContext db,
    IPasswordHasher<Patient> hasher,
    IHttpContextAccessor httpContextAccessor) : IPatientAccountService
{
    public async Task<AccountResult> RegisterAsync(PatientRegistrationForm form, CancellationToken ct = default)
    {
        var email = form.Email.Trim().ToLowerInvariant();
        var ehic = form.EhicNumber.Trim().ToUpperInvariant();

        if (await db.Patients.AnyAsync(p => p.Email == email, ct))
        {
            return AccountResult.Fail("An account with this email already exists.");
        }

        if (await db.Patients.AnyAsync(p => p.EhicNumber == ehic, ct))
        {
            return AccountResult.Fail("An account with this EHIC number already exists.");
        }

        var patient = new Patient
        {
            FirstName = form.FirstName.Trim(),
            LastName = form.LastName.Trim(),
            DateOfBirth = form.DateOfBirth!.Value,
            EhicNumber = ehic,
            Email = email,
            PasswordHash = string.Empty,
            Role = UserRole.Patient,
        };
        patient.PasswordHash = hasher.HashPassword(patient, form.Password);

        db.Patients.Add(patient);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return AccountResult.Fail("An account with this email or EHIC number already exists.");
        }

        var principal = BuildPrincipal(patient);
        await SignInAsync(principal);
        return AccountResult.Ok();
    }

    public async Task<AccountResult> LoginAsync(PatientLoginForm form, CancellationToken ct = default)
    {
        var patient = await ValidateCredentialsAsync(form, ct);
        if (patient is null)
        {
            return AccountResult.Fail("Invalid email or password.");
        }

        var principal = BuildPrincipal(patient);
        await SignInAsync(principal);
        return AccountResult.Ok();
    }

    public Task LogoutAsync() => SignOutAsync();

    public async Task<Patient?> ValidateCredentialsAsync(PatientLoginForm form, CancellationToken ct = default)
    {
        var email = form.Email.Trim().ToLowerInvariant();
        var patient = await db.Patients.SingleOrDefaultAsync(p => p.Email == email, ct);
        if (patient is null)
        {
            return null;
        }

        var result = hasher.VerifyHashedPassword(patient, patient.PasswordHash, form.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            patient.PasswordHash = hasher.HashPassword(patient, form.Password);
            await db.SaveChangesAsync(ct);
        }

        return patient;
    }

    public ClaimsPrincipal BuildPrincipal(Patient patient)
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, patient.Id.ToString()),
            new(ClaimTypes.Name, patient.Email),
            new(ClaimTypes.GivenName, patient.FirstName),
            new(ClaimTypes.Surname, patient.LastName),
            new(ClaimTypes.Role, patient.Role.ToString()),
        ];

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }

    public Task SignInAsync(ClaimsPrincipal principal) =>
        RequireHttpContext().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    public Task SignOutAsync() =>
        RequireHttpContext().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    private HttpContext RequireHttpContext() =>
        httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HttpContext — sign-in/out must happen during an HTTP request.");
}

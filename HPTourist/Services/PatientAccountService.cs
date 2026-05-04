using HPTourist.Data.DTOs;
using HPTourist.Data.Models;
using HPTourist.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HPTourist.Services;

public class PatientAccountService : IPatientAccountService
{
    private readonly DatabaseContext _db;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PatientAccountService(
        DatabaseContext db,
        IPasswordHasher<User> hasher,
        IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _hasher = hasher;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AccountResult> RegisterAsync(PatientRegistrationForm form, CancellationToken ct = default)
    {
        var email = form.Email.Trim().ToLowerInvariant();
        var ehic = form.EhicNumber.Trim().ToUpperInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
        {
            return AccountResult.Fail("An account with this email already exists.");
        }

        if (await _db.EHICs.AnyAsync(e => e.EncryptedEHICNumber == ehic, ct))
        {
            return AccountResult.Fail("An account with this EHIC number already exists.");
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var patient = new Patient
        {
            FirstName = form.FirstName.Trim(),
            LastName = form.LastName.Trim(),
            DateOfBirth = System.DateTime.SpecifyKind(form.DateOfBirth!.Value.ToDateTime(TimeOnly.MinValue), System.DateTimeKind.Utc),
            Gender = form.Gender!.Value,
            PracticeId = SeededIds.TouristDoctorAmsterdamPractice,
            EHIC = new EHIC
            {
                EncryptedEHICNumber = ehic,
                ExpiryDate = System.DateTime.SpecifyKind(form.EhicExpiryDate!.Value, System.DateTimeKind.Utc),
            },
        };
        _db.Patients.Add(patient);

        var user = new User
        {
            Email = email,
            Role = UserRole.Patient,
            PatientId = patient.Id,
            PasswordHash = string.Empty,
        };
        user.PasswordHash = _hasher.HashPassword(user, form.Password);
        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (DbUpdateException)
        {
            return AccountResult.Fail("An account with this email or EHIC number already exists.");
        }

        user.Patient = patient;
        var principal = BuildPrincipal(user);
        await SignInAsync(principal);
        return AccountResult.Ok();
    }

    public async Task<AccountResult> LoginAsync(PatientLoginForm form, CancellationToken ct = default)
    {
        var user = await ValidateCredentialsAsync(form, ct);
        if (user is null)
        {
            return AccountResult.Fail("Invalid email or password.");
        }

        var principal = BuildPrincipal(user);
        await SignInAsync(principal);
        return AccountResult.Ok();
    }

    public Task LogoutAsync() => SignOutAsync();

    public async Task<User?> ValidateCredentialsAsync(PatientLoginForm form, CancellationToken ct = default)
    {
        var email = form.Email.Trim().ToLowerInvariant();
        var user = await _db.Users
            .Include(u => u.Patient)
            .Include(u => u.Employee)
            .SingleOrDefaultAsync(u => u.Email == email, ct);
        if (user is null)
        {
            return null;
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, form.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _hasher.HashPassword(user, form.Password);
            await _db.SaveChangesAsync(ct);
        }

        return user;
    }

    public ClaimsPrincipal BuildPrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
        };

        if (user.Patient is not null)
        {
            claims.Add(new Claim("PatientId", user.Patient.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.GivenName, user.Patient.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname, user.Patient.LastName));
        }
        else if (user.Employee is not null)
        {
            claims.Add(new Claim("EmployeeId", user.Employee.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.GivenName, user.Employee.Name));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }

    public Task SignInAsync(ClaimsPrincipal principal) =>
        RequireHttpContext().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    public Task SignOutAsync() =>
        RequireHttpContext().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    private HttpContext RequireHttpContext() =>
        _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HttpContext — sign-in/out must happen during an HTTP request.");
}

using System.Security.Claims;
using System.Linq;
using HPTourist.Data.DTOs;
using HPTourist.Data.Models;
using HPTourist.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Services;

public sealed class PatientAccountService(
	DatabaseContext db,
	IPasswordHasher<User> hasher,
	IHttpContextAccessor httpContextAccessor,
	AuthenticationStateProvider authenticationStateProvider) : IPatientAccountService
{
	public async Task<AccountResult> RegisterAsync(PatientRegistrationForm form, CancellationToken ct = default)
	{
		var email = form.Email.Trim().ToLowerInvariant();
		var ehic = form.EhicNumber.Trim().ToUpperInvariant();

		if (await db.Users.AnyAsync(u => u.Email == email, ct))
		{
			return AccountResult.Fail("An account with this email already exists.");
		}

		if (await db.EHICs.AnyAsync(e => e.EncryptedEHICNumber == ehic, ct))
		{
			return AccountResult.Fail("An account with this EHIC number already exists.");
		}

		await using var tx = await db.Database.BeginTransactionAsync(ct);

		var patient = new Patient
		{
			FirstName = form.FirstName.Trim(),
			LastName = form.LastName.Trim(),
			DateOfBirth = DateTime.SpecifyKind(form.DateOfBirth!.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
			Gender = form.Gender!.Value,
			PracticeId = SeededIds.TouristDoctorAmsterdamPractice,
			EHIC = new EHIC
			{
				EncryptedEHICNumber = ehic,
				ExpiryDate = DateTime.SpecifyKind(form.EhicExpiryDate!.Value, DateTimeKind.Utc),
			},
		};
		db.Patients.Add(patient);

		var user = new User
		{
			Email = email,
			Role = UserRole.Patient,
			PatientId = patient.Id,
			PasswordHash = string.Empty,
		};
		user.PasswordHash = hasher.HashPassword(user, form.Password);
		db.Users.Add(user);

		try
		{
			await db.SaveChangesAsync(ct);
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

    public async Task<AccountResult> AddPatientInformationAsync(PatientInformationInputForm form, CancellationToken ct = default)
    {
        // Determine the current user from the HttpContext
        var userIdString = RequireHttpContext().User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return AccountResult.Fail("Not authenticated.");
        }

        var user = await db.Users
            .Include(u => u.Patient)
                .ThenInclude(p => p.Allergies)
            .SingleOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null || user.Patient is null)
        {
            return AccountResult.Fail("No patient associated with the current account.");
        }

        var patient = user.Patient;

        // Update basic fields
        patient.FirstName = form.FirstName.Trim();
        patient.LastName = form.LastName.Trim();
        if (form.DateOfBirth is not null)
        {
            patient.DateOfBirth = DateTime.SpecifyKind(form.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        }
        if (form.BloodType is not null)
        {
            patient.BloodType = form.BloodType.Value;
        }
        if (form.RhFactor is not null)
        {
            patient.RhFactor = form.RhFactor.Value;
        }
        patient.Weight = form.Weight;

        // Replace allergies: remove any existing and add the new ones provided
        if (patient.Allergies is not null && patient.Allergies.Any())
        {
            db.Allergies.RemoveRange(patient.Allergies);
            patient.Allergies.Clear();
        }

        var newAllergies = form.Allergies
            .Where(a => !string.IsNullOrWhiteSpace(a.Substance))
            .Select(a => new Allergy
            {
                Substance = a.Substance.Trim(),
                Reaction = string.IsNullOrWhiteSpace(a.Reaction) ? null : a.Reaction.Trim(),
            })
            .ToList();

        foreach (var a in newAllergies)
        {
            patient.Allergies.Add(a);
        }

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return AccountResult.Fail("Failed to save patient information.");
        }

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

	public async Task<AccountResult> ChangePasswordAsync(ChangePasswordForm form, CancellationToken ct = default)
	{
		AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
		ClaimsPrincipal principal = authState.User;

		if (principal.Identity?.IsAuthenticated != true)
			return AccountResult.Fail("Not authenticated.");

		Guid userId = Guid.Parse(principal.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
		User? user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId, ct);

		if (user is null)
			return AccountResult.Fail("User not found.");

		user.PasswordHash = hasher.HashPassword(user, form.NewPassword);
		await db.SaveChangesAsync(ct);

		return AccountResult.Ok();
	}

	public Task LogoutAsync() => SignOutAsync();

	public async Task<User?> ValidateCredentialsAsync(PatientLoginForm form, CancellationToken ct = default)
	{
		var email = form.Email.Trim().ToLowerInvariant();
		var user = await db.Users
			.Include(u => u.Patient)
			.Include(u => u.Employee)
			.SingleOrDefaultAsync(u => u.Email == email, ct);
		if (user is null)
		{
			return null;
		}

		var result = hasher.VerifyHashedPassword(user, user.PasswordHash, form.Password);
		if (result == PasswordVerificationResult.Failed)
		{
			return null;
		}

		if (result == PasswordVerificationResult.SuccessRehashNeeded)
		{
			user.PasswordHash = hasher.HashPassword(user, form.Password);
			await db.SaveChangesAsync(ct);
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
		httpContextAccessor.HttpContext
			?? throw new InvalidOperationException("No active HttpContext — sign-in/out must happen during an HTTP request.");
}

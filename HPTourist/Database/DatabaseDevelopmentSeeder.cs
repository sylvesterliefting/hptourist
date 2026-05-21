using HPTourist.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Database;

public static class DatabaseDevelopmentSeeder
{
    public const string DemoPatientEmail = "demo.patient@hptourist.local";
    public const string DemoPatientPassword = "Password123!";

    public static async Task SeedAsync(
        DatabaseContext context,
        IPasswordHasher<User> passwordHasher,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (await context.Users.AnyAsync(u => u.Email == DemoPatientEmail, cancellationToken))
        {
            logger.LogInformation("Development demo patient already exists");
            return;
        }

        var patient = new Patient
        {
            FirstName = "Demo",
            LastName = "Patient",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Gender = Gender.Unknown,
            PracticeId = SeededIds.TouristDoctorAmsterdamPractice,
            EHIC = new EHIC
            {
                EncryptedEHICNumber = "DEMOEHIC000000000001",
                ExpiryDate = new DateTime(2035, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
        };

        var user = new User
        {
            Email = DemoPatientEmail,
            Role = UserRole.Patient,
            PatientId = patient.Id,
            PasswordHash = string.Empty,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, DemoPatientPassword);

        context.Patients.Add(patient);
        context.Users.Add(user);

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Development demo patient created");
    }
}

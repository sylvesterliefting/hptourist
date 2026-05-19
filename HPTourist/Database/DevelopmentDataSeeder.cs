using HPTourist.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace HPTourist.Database;

internal static class DevelopmentDataSeeder
{
    private const string PatientEmail = "patient@hptourist.com";
    private const string EmployeeEmail = "huisarts@hptourist.com";
    private const string Password = "chipsoft";
    private const string EhicNumber = "PL123456789012345678";

    public static void SeedUserIfMissing(
        DatabaseContext db,
        IPasswordHasher<User> passwordHasher,
        ILogger logger)
    {
        SeedPatientIfMissing(db, passwordHasher, logger);
        SeedEmployeeIfMissing(db, passwordHasher, logger);
        SeedPrescriptionIfMissing(db, logger);
    }

    private static void SeedPatientIfMissing(
        DatabaseContext db,
        IPasswordHasher<User> passwordHasher,
        ILogger logger)
    {
        if (db.Users.Any(user => user.Email == PatientEmail))
        {
            logger.LogInformation("Development patient user {Email} already exists", PatientEmail);
            return;
        }

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Chipsoft",
            LastName = "Patient",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Gender = Gender.Unknown,
            PracticeId = SeededIds.TouristDoctorAmsterdamPractice,
            EHIC = new EHIC
            {
                EncryptedEHICNumber = EhicNumber,
                ExpiryDate = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = PatientEmail,
            Role = UserRole.Patient,
            PatientId = patient.Id,
            Patient = patient,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = string.Empty,
        };

        user.PasswordHash = passwordHasher.HashPassword(user, Password);

        db.Patients.Add(patient);
        db.Users.Add(user);
        db.SaveChanges();

        logger.LogInformation("Development patient user {Email} created", PatientEmail);
    }

    private static void SeedEmployeeIfMissing(
        DatabaseContext db,
        IPasswordHasher<User> passwordHasher,
        ILogger logger)
    {
        if (db.Users.Any(user => user.Email == EmployeeEmail))
        {
            logger.LogInformation("Development employee user {Email} already exists", EmployeeEmail);
            return;
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Tourist Doctor",
            Role = Role.GeneralPractitioner,
            PracticeId = SeededIds.TouristDoctorAmsterdamPractice,
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = EmployeeEmail,
            Role = UserRole.Employee,
            EmployeeId = employee.Id,
            Employee = employee,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = string.Empty,
        };

        user.PasswordHash = passwordHasher.HashPassword(user, Password);

        db.Employees.Add(employee);
        db.Users.Add(user);
        db.SaveChanges();

        logger.LogInformation("Development employee user {Email} created", EmployeeEmail);
    }

    private static void SeedPrescriptionIfMissing(DatabaseContext db, ILogger logger)
    {
        var patient = db.Patients
            .OrderBy(patient => patient.DateOfBirth)
            .FirstOrDefault(patient => patient.FirstName == "Chipsoft" && patient.LastName == "Patient");

        var employee = db.Employees
            .OrderBy(employee => employee.Name)
            .FirstOrDefault(employee => employee.Name == "Tourist Doctor");

        if (patient is null || employee is null)
        {
            logger.LogWarning("Development prescription was not created because the seeded patient or employee is missing");
            return;
        }

        var alreadyHasPrescription = db.Prescriptions.Any(prescription => prescription.PatientId == patient.Id);
        if (alreadyHasPrescription)
        {
            logger.LogInformation("Development prescription for {Email} already exists", PatientEmail);
            return;
        }

        var prescriptionRequest = new PrescriptionRequest
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            Type = PrescriptionRequest.RequestType.RepeatPrescription,
            RequestStatus = PrescriptionRequest.Status.Processed,
            Date = DateTime.UtcNow.AddDays(-14),
            Medicines =
            [
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    Name = "Paracetamol",
                    ActiveSubstance = "Paracetamol",
                    AtcCode = "N02BE01",
                    PharmaceuticalForm = "Tablet",
                },
            ],
        };

        var prescription = new Prescription
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            EmployeeId = employee.Id,
            PrescriptionRequestId = prescriptionRequest.Id,
            PrescriptionRequest = prescriptionRequest,
            Date = DateTime.UtcNow.AddDays(-14),
            Medicines =
            [
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    Name = "Paracetamol",
                    ActiveSubstance = "Paracetamol",
                    AtcCode = "N02BE01",
                    PharmaceuticalForm = "Tablet",
                },
            ],
        };

        db.PrescriptionRequests.Add(prescriptionRequest);
        db.Prescriptions.Add(prescription);
        db.SaveChanges();

        logger.LogInformation("Development prescription for {Email} created", PatientEmail);
    }
}

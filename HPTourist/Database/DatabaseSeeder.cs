using HPTourist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Database;

public class DatabaseSeeder
{
    public static void SeedDatabase(IServiceProvider serviceProvider)
    {
        using DatabaseContext databaseContext = new DatabaseContext(serviceProvider.GetRequiredService<DbContextOptions<DatabaseContext>>()) ?? throw new NullReferenceException("DatabaseContext is null.");
        
        var language = new Language()
        {
            Name = "English"
        };

        var practice = new Practice()
        {
            Name = "Test Practice",
            Address = "Teststreet 1"
        };

        var patient = new Patient()
        {
            FirstName = "Test",
            LastName = "Kees",
            Email = "test",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            Gender = Gender.Unknown,
            Practice = practice,
            PreferredLanguage = language,
        };

        var medicine = new Medicine()
        {
            Name = "Test",
            AtcCode = "t1e2s3t4",
            ActiveSubstance = "Test substance",
            PharmaceuticalForm = "Test form",
        };

        var prescriptionRequest = new PrescriptionRequest()
        {
            Patient = patient,
            Medicines = [medicine],
            RequestStatus = PrescriptionRequest.Status.Pending,
            Date = DateTime.UtcNow
        };

        if (!databaseContext.Languages.Any())
        {
            databaseContext.Languages.Add(language);
        }

        if (!databaseContext.Practices.Any())
        {
            databaseContext.Practices.Add(practice);
        }

        if (!databaseContext.Patients.Any())
        {
            databaseContext.Patients.Add(patient);
        }

        if (!databaseContext.PrescriptionRequests.Any())
        {
            databaseContext.PrescriptionRequests.AddRange(prescriptionRequest);
        }

        databaseContext.SaveChanges();
    }

}
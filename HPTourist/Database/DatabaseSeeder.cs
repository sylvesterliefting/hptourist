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

        databaseContext.Languages.Add(language);

        var practice = new Practice()
        {
            Name = "Test Practice",
            Address = "Teststreet 1"
        };

        databaseContext.Practices.Add(practice);

        var patient = new Patient()
        {
            FirstName = "Test",
            LastName = "Kees",
            Email = "test",
            DateOfBirth = DateTime.Now.AddYears(-20),
            Gender = Gender.Unknown,
            Practice = practice,
            PreferredLanguage = language,
        };

        databaseContext.Patients.Add(patient);

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
            Date = DateTime.Now
        };

        databaseContext.PrescriptionRequests.AddRange(prescriptionRequest);

        databaseContext.SaveChanges();
    }

}

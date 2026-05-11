using HPTourist.Data.Models;
using HPTourist.Database;
using HPTourist.Features.MedicationOverview;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Features.MedicationOverview.Infrastructure;

public sealed class MedicationOverviewQueryService(
    DatabaseContext dbContext,
    TimeProvider timeProvider) : IMedicationOverviewService
{
    private const string DefaultPracticeName = "Huisartsenpraktijk Tourist Doctor Amsterdam";
    private const string PrescribedMedicationTitle = "Voorgeschreven medicatie";
    private const string SelfReportedMedicationTitle = "Zelf toegevoegde medicatie";

    public async Task<MedicationOverviewDocument?> GetForPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var patient = await dbContext.Patients
            .AsNoTracking()
            .Include(p => p.Practice)
            .SingleOrDefaultAsync(p => p.Id == patientId, cancellationToken);

        if (patient is null)
        {
            return null;
        }

        var prescriptions = await dbContext.Prescriptions
            .AsNoTracking()
            .Where(p => p.PatientId == patientId)
            .Include(p => p.Medicines)
            .OrderByDescending(p => p.Date)
            .ToListAsync(cancellationToken);

        var prescriptionRequests = await dbContext.PrescriptionRequests
            .AsNoTracking()
            .Where(r => r.PatientId == patientId)
            .Include(r => r.Medicines)
            .OrderByDescending(r => r.Date)
            .ToListAsync(cancellationToken);

        return new MedicationOverviewDocument(
            new PatientOverview(
                $"{patient.FirstName} {patient.LastName}",
                patient.DateOfBirth,
                string.IsNullOrWhiteSpace(patient.Practice?.Name)
                    ? DefaultPracticeName
                    : patient.Practice.Name),
            timeProvider.GetLocalNow(),
            [
                new MedicationOverviewSection(
                    PrescribedMedicationTitle,
                    prescriptions.SelectMany(ToPrescribedRows).ToList()),
                new MedicationOverviewSection(
                    SelfReportedMedicationTitle,
                    prescriptionRequests.SelectMany(ToPrescriptionRequestRows).ToList())
            ]);
    }

    private static IEnumerable<MedicationOverviewRow> ToPrescribedRows(Prescription prescription)
    {
        return prescription.Medicines.Select(medicine => new MedicationOverviewRow(
            prescription.Date,
            medicine.Name,
            medicine.ActiveSubstance,
            medicine.PharmaceuticalForm,
            NormalizeAtcCode(medicine.AtcCode),
            "Voorgeschreven"));
    }

    private static IEnumerable<MedicationOverviewRow> ToPrescriptionRequestRows(PrescriptionRequest request)
    {
        return request.Medicines.Select(medicine => new MedicationOverviewRow(
            request.Date,
            medicine.Name,
            medicine.ActiveSubstance,
            medicine.PharmaceuticalForm,
            NormalizeAtcCode(medicine.AtcCode),
            GetStatusText(request.RequestStatus)));
    }

    private static string NormalizeAtcCode(string? atcCode)
    {
        return string.IsNullOrWhiteSpace(atcCode) ? "-" : atcCode.Trim();
    }

    private static string GetStatusText(PrescriptionRequest.Status status)
    {
        return status switch
        {
            PrescriptionRequest.Status.Pending => "In behandeling",
            PrescriptionRequest.Status.Processed => "Goedgekeurd",
            PrescriptionRequest.Status.Rejected => "Afgewezen",
            _ => "Onbekend"
        };
    }
}

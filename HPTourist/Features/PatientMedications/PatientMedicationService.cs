using HPTourist.Data.DTOs;
using HPTourist.Data.Models;
using HPTourist.Database;
using HPTourist.Services;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Features.PatientMedications;

public sealed class PatientMedicationService(
    DatabaseContext db,
    TimeProvider timeProvider) : IPatientMedicationService
{
    public async Task<PatientMedicationOverview> GetOverviewAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var prescriptions = await db.Prescriptions
            .AsNoTracking()
            .Where(prescription => prescription.PatientId == patientId)
            .Include(prescription => prescription.Medicines)
            .OrderByDescending(prescription => prescription.Date)
            .ToListAsync(cancellationToken);

        var prescribedMedications = prescriptions
            .SelectMany(prescription => prescription.Medicines.Select(medicine => new PrescribedMedicationListItem(
                medicine.Id,
                medicine.Name,
                medicine.ActiveSubstance,
                medicine.PharmaceuticalForm,
                prescription.Date)))
            .ToList();

        var selfAddedMedications = await db.PatientMedications
            .AsNoTracking()
            .Where(medication => medication.PatientId == patientId)
            .OrderByDescending(medication => medication.CreatedAt)
            .Select(medication => new PatientMedicationListItem(
                medication.Id,
                medication.Name,
                medication.ActiveSubstance,
                medication.AtcCode,
                medication.PharmaceuticalForm,
                medication.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PatientMedicationOverview(prescribedMedications, selfAddedMedications);
    }

    public async Task<PatientMedicationDetails?> GetMedicationAsync(
        Guid patientId,
        Guid medicationId,
        CancellationToken cancellationToken = default)
    {
        return await db.PatientMedications
            .AsNoTracking()
            .Where(medication => medication.Id == medicationId && medication.PatientId == patientId)
            .Select(medication => new PatientMedicationDetails(
                medication.Id,
                medication.Name,
                medication.ActiveSubstance,
                medication.AtcCode,
                medication.PharmaceuticalForm))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<AccountResult> AddMedicationAsync(
        Guid patientId,
        MedicineForm form,
        CancellationToken cancellationToken = default)
    {
        var patientExists = await db.Patients
            .AnyAsync(patient => patient.Id == patientId, cancellationToken);

        if (!patientExists)
        {
            return AccountResult.Fail("Patient kon niet worden gevonden.");
        }

        db.PatientMedications.Add(new PatientMedication
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Name = form.Name.Trim(),
            ActiveSubstance = form.ActiveSubstance.Trim(),
            AtcCode = NormalizeAtcCode(form.AtcCode),
            PharmaceuticalForm = form.PharmaceuticalForm.Trim(),
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        });

        await db.SaveChangesAsync(cancellationToken);
        return AccountResult.Ok();
    }

    public async Task<AccountResult> UpdateMedicationAsync(
        Guid patientId,
        Guid medicationId,
        MedicineForm form,
        CancellationToken cancellationToken = default)
    {
        var medication = await db.PatientMedications
            .SingleOrDefaultAsync(
                item => item.Id == medicationId && item.PatientId == patientId,
                cancellationToken);

        if (medication is null)
        {
            return AccountResult.Fail("Medicatie kon niet worden gevonden.");
        }

        medication.Name = form.Name.Trim();
        medication.ActiveSubstance = form.ActiveSubstance.Trim();
        medication.AtcCode = NormalizeAtcCode(form.AtcCode);
        medication.PharmaceuticalForm = form.PharmaceuticalForm.Trim();

        await db.SaveChangesAsync(cancellationToken);
        return AccountResult.Ok();
    }

    public async Task<AccountResult> DeleteMedicationAsync(
        Guid patientId,
        Guid medicationId,
        CancellationToken cancellationToken = default)
    {
        var medication = await db.PatientMedications
            .SingleOrDefaultAsync(
                item => item.Id == medicationId && item.PatientId == patientId,
                cancellationToken);

        if (medication is null)
        {
            return AccountResult.Fail("Medicatie kon niet worden gevonden.");
        }

        db.PatientMedications.Remove(medication);
        await db.SaveChangesAsync(cancellationToken);
        return AccountResult.Ok();
    }

    private static string NormalizeAtcCode(string? atcCode)
    {
        return atcCode?.Trim().ToUpperInvariant() ?? string.Empty;
    }
}

using HPTourist.Data.Models;
using HPTourist.Database;
using HPTourist.Services;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Features.RepeatPrescriptions;

public sealed class RepeatPrescriptionRequestService(
    DatabaseContext db,
    TimeProvider timeProvider) : IRepeatPrescriptionRequestService
{
    private const int MaximumMedicinesPerRequest = 20;

    public async Task<IReadOnlyList<RepeatPrescriptionMedicineOption>> GetMedicineOptionsAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var prescriptions = await db.Prescriptions
            .AsNoTracking()
            .Where(prescription => prescription.PatientId == patientId)
            .Include(prescription => prescription.Medicines)
            .OrderByDescending(prescription => prescription.Date)
            .ToListAsync(cancellationToken);

        return prescriptions
            .SelectMany(prescription => prescription.Medicines.Select(medicine => new
            {
                Medicine = medicine,
                prescription.Date,
            }))
            .Select(item => new RepeatPrescriptionMedicineOption(
                item.Medicine.Id,
                item.Medicine.Name,
                item.Medicine.ActiveSubstance,
                item.Medicine.AtcCode,
                item.Medicine.PharmaceuticalForm,
                item.Date))
            .ToList();
    }

    public async Task<AccountResult> CreateRequestAsync(
        Guid patientId,
        IReadOnlyCollection<Guid> medicineIds,
        CancellationToken cancellationToken = default)
    {
        var selectedMedicineIds = medicineIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        if (selectedMedicineIds.Length == 0)
        {
            return AccountResult.Fail("Selecteer minimaal een medicijn voor het herhaalrecept.");
        }

        if (selectedMedicineIds.Length > MaximumMedicinesPerRequest)
        {
            return AccountResult.Fail($"Een aanvraag mag maximaal {MaximumMedicinesPerRequest} medicijnen bevatten.");
        }

        var patientExists = await db.Patients
            .AnyAsync(patient => patient.Id == patientId, cancellationToken);

        if (!patientExists)
        {
            return AccountResult.Fail("Patient kon niet worden gevonden.");
        }

        var prescribedMedicines = await GetPrescribedMedicinesAsync(
            patientId,
            selectedMedicineIds,
            cancellationToken);

        if (prescribedMedicines.Count != selectedMedicineIds.Length)
        {
            return AccountResult.Fail("Een of meer geselecteerde medicijnen horen niet bij uw dossier.");
        }

        var request = new PrescriptionRequest
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Date = timeProvider.GetUtcNow().UtcDateTime,
            Type = PrescriptionRequest.RequestType.RepeatPrescription,
            RequestStatus = PrescriptionRequest.Status.Pending,
            Medicines = prescribedMedicines
                .Select(CloneMedicineForRequest)
                .ToList(),
        };

        db.PrescriptionRequests.Add(request);
        await db.SaveChangesAsync(cancellationToken);

        return AccountResult.Ok();
    }

    private async Task<List<Medicine>> GetPrescribedMedicinesAsync(
        Guid patientId,
        IReadOnlyCollection<Guid> medicineIds,
        CancellationToken cancellationToken)
    {
        var prescriptions = await db.Prescriptions
            .AsNoTracking()
            .Where(prescription => prescription.PatientId == patientId)
            .Include(prescription => prescription.Medicines)
            .ToListAsync(cancellationToken);

        return prescriptions
            .SelectMany(prescription => prescription.Medicines)
            .Where(medicine => medicineIds.Contains(medicine.Id))
            .ToList();
    }

    private static Medicine CloneMedicineForRequest(Medicine medicine)
    {
        return new Medicine
        {
            Id = Guid.NewGuid(),
            Name = medicine.Name,
            ActiveSubstance = medicine.ActiveSubstance,
            AtcCode = medicine.AtcCode,
            PharmaceuticalForm = medicine.PharmaceuticalForm,
        };
    }
}

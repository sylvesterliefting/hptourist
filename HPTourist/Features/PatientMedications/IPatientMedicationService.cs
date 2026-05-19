using HPTourist.Data.DTOs;
using HPTourist.Services;

namespace HPTourist.Features.PatientMedications;

public interface IPatientMedicationService
{
    Task<PatientMedicationOverview> GetOverviewAsync(Guid patientId, CancellationToken cancellationToken = default);

    Task<PatientMedicationDetails?> GetMedicationAsync(
        Guid patientId,
        Guid medicationId,
        CancellationToken cancellationToken = default);

    Task<AccountResult> AddMedicationAsync(
        Guid patientId,
        MedicineForm form,
        CancellationToken cancellationToken = default);

    Task<AccountResult> UpdateMedicationAsync(
        Guid patientId,
        Guid medicationId,
        MedicineForm form,
        CancellationToken cancellationToken = default);

    Task<AccountResult> DeleteMedicationAsync(
        Guid patientId,
        Guid medicationId,
        CancellationToken cancellationToken = default);
}

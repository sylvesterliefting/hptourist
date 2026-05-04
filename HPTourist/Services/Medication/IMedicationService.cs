using HPTourist.Data.DTOs.Medication;

namespace HPTourist.Services.Medication;

public interface IMedicationService
{
    Task<List<PrescriptionDto>> GetPrescriptionsAsync(Guid patientId, CancellationToken ct = default);
    Task<List<PrescriptionRequestDto>> GetPrescriptionRequestsAsync(Guid patientId, CancellationToken ct = default);
    Task<PrescriptionRequestDto?> GetPrescriptionRequestByMedicineIdAsync(Guid patientId, Guid medicineId, CancellationToken ct = default);
    Task AddPrescriptionRequestAsync(Guid patientId, MedicineForm form, CancellationToken ct = default);
    Task UpdateMedicineAsync(Guid patientId, Guid medicineId, MedicineForm form, CancellationToken ct = default);
    Task DeleteMedicineAsync(Guid patientId, Guid prescriptionRequestId, Guid medicineId, CancellationToken ct = default);
}

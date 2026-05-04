using HPTourist.Data.DTOs.Medication;
using HPTourist.Data.Models;

namespace HPTourist.Services.Medication;

public interface IMedicationMapper
{
    PrescriptionDto MapToDto(Prescription entity);
    PrescriptionRequestDto MapToDto(PrescriptionRequest entity);
    MedicineDto MapToDto(Medicine entity);
    Medicine MapToEntity(MedicineForm form);
    void ApplyToEntity(Medicine medicine, MedicineForm form);
}

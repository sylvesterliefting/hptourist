using HPTourist.Data.DTOs.Medication;
using HPTourist.Data.Models;

namespace HPTourist.Services.Medication;

public class MedicationMapper : IMedicationMapper
{
    public PrescriptionDto MapToDto(Prescription entity)
    {
        return new(entity.Id, entity.Medicines.Select(MapToDto).ToList(), entity.Date);
    }

    public PrescriptionRequestDto MapToDto(PrescriptionRequest entity)
    {
        return new(entity.Id, entity.Medicines.Select(MapToDto).ToList(), entity.RequestStatus, entity.Date);
    }

    public MedicineDto MapToDto(Medicine entity)
    {
        return new(entity.Id, entity.Name, entity.ActiveSubstance, entity.AtcCode, entity.PharmaceuticalForm);
    }

    public Medicine MapToEntity(MedicineForm form)
    {
        Medicine medicine = new();
        ApplyToEntity(medicine, form);
        return medicine;
    }

    public void ApplyToEntity(Medicine medicine, MedicineForm form)
    {
        medicine.Name = form.Name.Trim();
        medicine.ActiveSubstance = form.ActiveSubstance.Trim();
        medicine.AtcCode = form.AtcCode?.Trim() ?? string.Empty;
        medicine.PharmaceuticalForm = form.PharmaceuticalForm.Trim();
    }
}

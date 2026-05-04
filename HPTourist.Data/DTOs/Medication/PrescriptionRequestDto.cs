using HPTourist.Data.Models;

namespace HPTourist.Data.DTOs.Medication;

public class PrescriptionRequestDto
{
    public Guid Id { get; init; }
    public List<MedicineDto> Medicines { get; init; } = [];
    public PrescriptionRequest.Status Status { get; init; }
    public DateTime Date { get; init; }

    public PrescriptionRequestDto() { }

    public PrescriptionRequestDto(Guid id, List<MedicineDto> medicines, PrescriptionRequest.Status status, DateTime date)
    {
        Id = id;
        Medicines = medicines;
        Status = status;
        Date = date;
    }
}

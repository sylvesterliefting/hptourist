namespace HPTourist.Data.DTOs.Medication;

public class PrescriptionDto
{
    public Guid Id { get; init; }
    public List<MedicineDto> Medicines { get; init; } = [];
    public DateTime Date { get; init; }

    public PrescriptionDto() { }

    public PrescriptionDto(Guid id, List<MedicineDto> medicines, DateTime date)
    {
        Id = id;
        Medicines = medicines;
        Date = date;
    }
}

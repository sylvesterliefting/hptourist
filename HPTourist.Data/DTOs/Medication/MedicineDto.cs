namespace HPTourist.Data.DTOs.Medication;

public class MedicineDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ActiveSubstance { get; init; } = string.Empty;
    public string AtcCode { get; init; } = string.Empty;
    public string PharmaceuticalForm { get; init; } = string.Empty;

    public MedicineDto() { }

    public MedicineDto(Guid id, string name, string activeSubstance, string atcCode, string pharmaceuticalForm)
    {
        Id = id;
        Name = name;
        ActiveSubstance = activeSubstance;
        AtcCode = atcCode;
        PharmaceuticalForm = pharmaceuticalForm;
    }
}

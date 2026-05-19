namespace HPTourist.Data.Models;

public class PatientMedication
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string AtcCode { get; set; } = string.Empty;
    public string ActiveSubstance { get; set; } = default!;
    public string PharmaceuticalForm { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

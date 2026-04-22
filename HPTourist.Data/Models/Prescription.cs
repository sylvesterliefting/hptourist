namespace HPTourist.Data.Models;

public class Prescription
{
    public Guid Id { get; set; }
    public Patient Patient { get; set; } = default!;
    public PrescriptionRequest PrescriptionRequest { get; set; } = default!;
    public Employee Employee { get; set; } = default!;
    public List<Medicine> Medicines { get; set; } = default!; // Dutch medicine list 
    public DateTime Date { get; set; }
}

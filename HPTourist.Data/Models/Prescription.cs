namespace HPTourist.Data.Models;

public class Prescription
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;
    public Guid PrescriptionRequestId { get; set; }
    public PrescriptionRequest PrescriptionRequest { get; set; } = default!;
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    public List<Medicine> Medicines { get; set; } = []; // Dutch medicine list
    public DateTime Date { get; set; }
}

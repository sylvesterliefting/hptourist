namespace HPTourist.Data.Models;

public class PrescriptionRequest
{
    public Guid Id { get; set; }
    public Patient Patient { get; set; } = default!;
    public List<Medicine> Medicines { get; set; } = default!;
    public Status RequestStatus { get; set; }
    public DateTime Date { get; set; }
    public enum Status { Pending, Processed, Rejected }
}




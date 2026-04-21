namespace HPTourist.Models;


public class Prescription
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid PrescriptionRequestId { get; set; }
    public Guid DoctorId { get; set; }
    public List<Medicine> Medicines { get; set; } = default!; //Dutch medicine list 
    public DateTime Date { get; set; }
}

namespace HPTourist.Features.RepeatPrescriptions;

public sealed class RepeatPrescriptionRequestForm
{
    public HashSet<Guid> MedicineIds { get; } = [];
}

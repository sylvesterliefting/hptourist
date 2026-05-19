namespace HPTourist.Features.ContraIndications;

public sealed record ContraIndicationListItem(
    string ActiveSubstance,
    string MedicineNames,
    string PrescriptionDates,
    string? Text)
{
    public bool IsAvailable => !string.IsNullOrWhiteSpace(Text);
}

namespace HPTourist.Features.RepeatPrescriptions;

public sealed record RepeatPrescriptionMedicineOption(
    Guid MedicineId,
    string Name,
    string ActiveSubstance,
    string AtcCode,
    string PharmaceuticalForm,
    DateTime PrescribedAt);

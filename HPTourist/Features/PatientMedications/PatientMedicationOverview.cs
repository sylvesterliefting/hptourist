namespace HPTourist.Features.PatientMedications;

public sealed record PatientMedicationOverview(
    IReadOnlyList<PrescribedMedicationListItem> PrescribedMedications,
    IReadOnlyList<PatientMedicationListItem> SelfAddedMedications);

public sealed record PrescribedMedicationListItem(
    Guid MedicineId,
    string Name,
    string ActiveSubstance,
    string PharmaceuticalForm,
    DateTime PrescribedAt);

public sealed record PatientMedicationListItem(
    Guid Id,
    string Name,
    string ActiveSubstance,
    string AtcCode,
    string PharmaceuticalForm,
    DateTime CreatedAt);

public sealed record PatientMedicationDetails(
    Guid Id,
    string Name,
    string ActiveSubstance,
    string AtcCode,
    string PharmaceuticalForm);

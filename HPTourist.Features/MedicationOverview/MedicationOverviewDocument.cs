namespace HPTourist.Features.MedicationOverview;

public sealed record MedicationOverviewDocument(
    PatientOverview Patient,
    DateTimeOffset GeneratedAt,
    IReadOnlyList<MedicationOverviewSection> Sections);

public sealed record PatientOverview(
    string FullName,
    DateTime DateOfBirth,
    string PracticeName);

public sealed record MedicationOverviewSection(
    string Title,
    IReadOnlyList<MedicationOverviewRow> Rows);

public sealed record MedicationOverviewRow(
    DateTime Date,
    string Name,
    string ActiveSubstance,
    string PharmaceuticalForm,
    string AtcCode,
    string Status);


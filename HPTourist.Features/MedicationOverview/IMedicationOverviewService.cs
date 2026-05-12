namespace HPTourist.Features.MedicationOverview;

public interface IMedicationOverviewService
{
    Task<MedicationOverviewDocument?> GetForPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
}


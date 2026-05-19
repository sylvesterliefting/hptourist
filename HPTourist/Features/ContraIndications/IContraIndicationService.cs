namespace HPTourist.Features.ContraIndications;

public interface IContraIndicationService
{
    Task<IReadOnlyList<ContraIndicationListItem>> GetForPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}

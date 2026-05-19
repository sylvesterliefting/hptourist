using HPTourist.Services;

namespace HPTourist.Features.RepeatPrescriptions;

public interface IRepeatPrescriptionRequestService
{
    Task<IReadOnlyList<RepeatPrescriptionMedicineOption>> GetMedicineOptionsAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<AccountResult> CreateRequestAsync(
        Guid patientId,
        IReadOnlyCollection<Guid> medicineIds,
        CancellationToken cancellationToken = default);
}

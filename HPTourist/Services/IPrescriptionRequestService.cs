using HPTourist.Data.Models;

namespace HPTourist.Services;

public interface IPrescriptionRequestService
{
    Task<List<PrescriptionRequest>> GetPrescriptionRequestsByStatusesAsync(List<PrescriptionRequest.Status>? status = default, CancellationToken ct = default);
}
